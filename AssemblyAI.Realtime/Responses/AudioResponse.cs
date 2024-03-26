using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Responses
{
    public class AudioResponse : BaseResponse
    {
        /// <summary>
        /// Start time of audio sample relative to session start, in milliseconds.
        /// </summary>
        [JsonProperty("audio_start")]
        public long AudioStart { get; internal set; }

        /// <summary>
        /// End time of audio sample relative to session start, in milliseconds.
        /// </summary>
        [JsonProperty("audio_end")]
        public long AudioEnd { get; internal set; }

        /// <summary>
        /// The confidence score of the entire transcription, between 0 and 1.
        /// </summary>
        [JsonProperty("confidence")]
        public decimal Confidence { get; internal set; }

        /// <summary>
        /// The partial/final transcript for your audio.
        /// </summary>
        [JsonProperty("text")]
        public string? Text { get; internal set; }

        /// <summary>
        /// An array of objects, with the information for each word in the transcription text. Includes the start/end time (in milliseconds) of the word, the confidence score of the word, and the text (i.e. the word itself).
        /// </summary>
        [JsonProperty("words")]
        public Word[]? Words { get; internal set; }

        /// <summary>
        /// The timestamp for the partial/final transcript.
        /// </summary>
        [JsonIgnore]
        public DateTime? Created { get; internal set; }

        /// <summary>
        /// Internal field used for serialization.
        /// </summary>
        [JsonProperty("created")]
        internal string? _Created
        {
            get => Created?.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
            set
            {
                if (value == null)
                {
                    Created = null;
                    return;
                }

                Created = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.None);
            }
        }

        /// <summary>
        /// Whether the text has been punctuated and cased.
        /// </summary>
        [JsonProperty("punctuated")]
        public bool Puncuated { get; internal set; } = false;

        /// <summary>
        /// Whether the text has been formatted (e.g. Dollar -> $)
        /// </summary>
        [JsonProperty("text_formatted")]
        public bool TextFormatted { get; internal set; } = false;

        public AudioResponse() : base(Enums.ResponseType.None) { }
    }
}
