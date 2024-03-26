using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Messages
{
    internal class EndUtteranceThreshold
    {
        /// <summary>
        /// The duration threshold in milliseconds. Default is 700.
        /// </summary>
        [JsonProperty("end_utterance_silence_threshold")]
        public int SilenceThreshold { get; private set; } = 700;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threshold">The duration threshold in milliseconds. Default is 700.</param>
        public EndUtteranceThreshold(int threshold = 700)
        {
            SilenceThreshold = threshold;
        }
    }
}
