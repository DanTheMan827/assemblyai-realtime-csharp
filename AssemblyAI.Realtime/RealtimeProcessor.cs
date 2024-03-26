using AssemblyAI.Realtime.Messages;
using AssemblyAI.Realtime.Responses;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;
using Websocket.Client;

namespace AssemblyAI.Realtime
{
    public class RealtimeProcessor : IDisposable
    {
        private Uri _apiEndpoint;
        private int _silenceThreshold;
        private int _bufferDuration;
        private int _sampleRate;
        private WebsocketClient? _webSocket;
        private WasapiCapture? _capture;
        private TemporaryToken _temporaryToken;

        /// <summary>
        /// Event raised when an audio response is received.
        /// </summary>
        public delegate void AudioResponseEvent(RealtimeProcessor sender, AudioResponse e);
        /// <summary>
        /// Event raised when a session begins.
        /// </summary>
        public delegate void SessionBeginsEvent(RealtimeProcessor sender, SessionBeginsResponse e);
        /// <summary>
        /// Event raised when the connection is disconnected.
        /// </summary>
        public delegate void DisconnectedEvent(RealtimeProcessor sender, DisconnectionInfo? e);
        /// <summary>
        /// Event raised when a session is terminated.
        /// </summary>
        public delegate void SessionTerminatedEvent(RealtimeProcessor sender, SessionTerminatedResponse e);
        /// <summary>
        /// Event raised when an unknown message is received.
        /// </summary>
        public delegate void UnknownMessageEvent(RealtimeProcessor sender, JObject e);

        /// <summary>
        /// Event raised when an audio response is received.
        /// </summary>
        public event AudioResponseEvent? OnAudioResponse;

        /// <summary>
        /// Event raised when a session begins.
        /// </summary>
        public event SessionBeginsEvent? OnSessionBegins;

        /// <summary>
        /// Event raised when the connection is disconnected.
        /// </summary>
        public event DisconnectedEvent? OnDisconnected;

        /// <summary>
        /// Event raised when a session is terminated.
        /// </summary>
        public event SessionTerminatedEvent OnSessionTerminated;

        /// <summary>
        /// Event raised when an unknown message is received.
        /// </summary>
        public event UnknownMessageEvent OnUnknownMessage;

        /// <summary>
        /// Indicates whether transcription is currently running.
        /// </summary>
        public bool TranscriptionRunning => _webSocket != null && _webSocket.IsRunning;

        /// <summary>
        /// Creates an instance of the RealtimeProcessor class.
        /// </summary>
        /// <param name="temporaryToken">The temporary token to use for authentication.</param>
        /// <param name="silenceThreshold">The duration threshold in milliseconds. Default is 700.</param>
        /// <param name="sampleRate">The audio sample rate (single channel).</param>
        /// <param name="bufferDuration">The length of the buffer in milliseconds that will be sent to AssemblyAI.</param>
        public RealtimeProcessor(TemporaryToken temporaryToken, int silenceThreshold = 700, int sampleRate = 44100, int bufferDuration = 450)
        {
            _silenceThreshold = silenceThreshold;
            _sampleRate = sampleRate;
            _bufferDuration = bufferDuration;
            _apiEndpoint = new Uri($"wss://api.assemblyai.com/v2/realtime/ws?sample_rate={sampleRate}&encoding=pcm_s16le&token={Uri.EscapeDataString(temporaryToken.Token)}");
            _temporaryToken = temporaryToken;
        }

        private async void Websocket_DisconnectHappened(DisconnectionInfo? info)
        {
            if (info != null)
            {
                info.CancelReconnection = true;
            }

            _capture?.StopRecording();
            _capture?.Dispose();
            _capture = null;

            _webSocket?.Dispose();
            _webSocket = null;

            OnDisconnected?.Invoke(this, info);
        }

        private async void Websocket_MessageReceived(ResponseMessage message)
        {
            var response = JObject.Parse(message.Text ?? "{}");

            switch (response["message_type"]?.ToString())
            {
                case "SessionBegins":
                    if (_webSocket != null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        await _webSocket.SendInstant(JsonConvert.SerializeObject(new EndUtteranceThreshold(_silenceThreshold)));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }

                    _capture = new WasapiCapture(WasapiCapture.GetDefaultCaptureDevice(), false, _bufferDuration);
                    _capture.WaveFormat = new WaveFormat(_sampleRate, 1);
                    _capture.DataAvailable += this.Capture_DataAvailable;
                    _capture.StartRecording();

                    await Task.Run(() => OnSessionBegins?.Invoke(this, response.ToObject<SessionBeginsResponse>()));
                    break;

                case "PartialTranscript":
                case "FinalTranscript":
                    await Task.Run(() => OnAudioResponse?.Invoke(this, response.ToObject<AudioResponse>()));
                    break;

                case "SessionTerminated":
                    await Task.Run(() => OnSessionTerminated?.Invoke(this, response.ToObject<SessionTerminatedResponse>()));

                    _webSocket?.Dispose();
                    _webSocket = null;

                    await Task.Run(() => OnDisconnected?.Invoke(this, null));
                    break;

                default:
                    await Task.Run(() => OnUnknownMessage?.Invoke(this, response));
                    break;
            }
        }

        /// <summary>
        /// Fired when new audio data is available.  This sends the audio data over the websocket connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            // Make sure our WebSocket is still running.
            if (Websocket_MessageReceived == null || !_webSocket.IsRunning)
            {
                // Our websocket is gone, stop the capture.
                _capture?.StopRecording();
                _capture?.Dispose();
                _capture = null;

                return;
            }

            // Send the audio data to the websocket.
            _webSocket.Send(e.Buffer);
        }

        /// <summary>
        /// Starts the transcription
        /// </summary>
        /// <exception cref="Exception"></exception>
        public async void StartTranscription()
        {
            if (TranscriptionRunning)
            {
                throw new Exception("Transcription is already running!");
            }

            if (DateTime.Now > _temporaryToken.ExpiresAt)
            {
                throw new Exception("The temporary token has expired and can no longer be used.");
            }

            // Dispose of any previous websocket that may exist, and setup a new one.
            _webSocket?.Dispose();
            _webSocket = new WebsocketClient(_apiEndpoint);
            _webSocket.IsReconnectionEnabled = false;
            _webSocket.MessageReceived.Subscribe(Websocket_MessageReceived);
            _webSocket.DisconnectionHappened.Subscribe(Websocket_DisconnectHappened);

            // Try to connect to the websocket, or fail.
            try
            {
                await _webSocket.StartOrFail();
            }
            catch (Exception ex)
            {
                _webSocket.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Stops the transcription and returns after the final response has been sent.
        /// </summary>
        public async void StopTranscription()
        {
            _capture?.StopRecording();
            _capture?.Dispose();
            _capture = null;
            _webSocket?.Send(JsonConvert.SerializeObject(new TerminateMessage()));
        }

        /// <summary>
        /// Immediately disposes the class and closes any open connection.
        /// </summary>
        public void Dispose()
        {
            _capture?.Dispose();
            _webSocket?.Dispose();
        }
    }
}
