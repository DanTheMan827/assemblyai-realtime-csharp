using AssemblyAI.Realtime.Enums;
using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Responses
{
    public class BaseResponse
    {
        [JsonIgnore]
        public ResponseType Type { get; internal set; } = ResponseType.None;

        [JsonProperty("message_type")]
        internal string _Type
        {
            get => Type.ToString().Split(".").Last();
            set
            {
                Type = value switch
                {
                    "SessionBegins" => ResponseType.SessionBegins,
                    "PartialTranscript" => ResponseType.PartialTranscript,
                    "FinalTranscript" => ResponseType.FinalTranscript,
                    "SessionTerminated" => ResponseType.SessionTerminated,
                    null => ResponseType.None,
                    _ => throw new ArgumentException(nameof(value))
                };
            }
        }

        public BaseResponse()
        {

        }

        public BaseResponse(ResponseType type)
        {
            this.Type = type;
        }

        public BaseResponse(string type)
        {
            _Type = type;
        }
    }
}
