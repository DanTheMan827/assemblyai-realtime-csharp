using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AssemblyAI.Realtime.Responses
{
    public class SessionBeginsResponse : BaseResponse
    {
        /// <summary>
        /// Unique identifier for the established session.
        /// </summary>
        [JsonProperty("session_id")]
        public string? SessionId { get; internal set; }

        /// <summary>
        /// Timestamp when this session will expire.
        /// </summary>
        [JsonIgnore]
        public DateTime? ExpiresAt { get; internal set; }

        /// <summary>
        /// Internal field used for serialization.
        /// </summary>
        [JsonProperty("expires_at")]
        internal string? _ExpiresAt
        {
            get => ExpiresAt?.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
            set
            {
                if (value == null)
                {
                    ExpiresAt = null;
                    return;
                }

                ExpiresAt = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.None);
            }
        }
        public SessionBeginsResponse() : base(Enums.ResponseType.SessionBegins) { }
    }
}
