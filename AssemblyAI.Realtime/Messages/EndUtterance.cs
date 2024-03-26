using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Messages
{
    /// <summary>
    /// To manually end an utterance, the client should send this as json.
    /// </summary>
    internal class EndUtterance
    {
        /// <summary>
        /// A boolean value to communicate that you wish to force the end of the utterance.
        /// </summary>
        [JsonProperty("force_end_utterance")]
        public bool ForceEndUtterance { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceEndUtterance">A boolean value to communicate that you wish to force the end of the utterance.  Manually ending an utterance immediately produces a final transcript.</param>
        public EndUtterance(bool forceEndUtterance = true)
        {
            ForceEndUtterance = forceEndUtterance;
        }
    }
}
