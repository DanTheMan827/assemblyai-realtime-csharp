using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Messages
{
    /// <summary>
    /// When you've completed your session, the client should send this encoded as json.
    /// </summary>
    internal struct TerminateMessage
    {
        /// <summary>
        /// A boolean value to communicate that you wish to end your streaming session forever.
        /// </summary>
        [JsonProperty("terminate_session")]
        public bool TerminateSession { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminateSession">A boolean value to communicate that you wish to end your streaming session forever.</param>
        public TerminateMessage(bool terminateSession = true)
        {
            this.TerminateSession = terminateSession;
        }
    }
}
