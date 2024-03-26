using Newtonsoft.Json;

namespace AssemblyAI.Realtime.Responses
{
    public class Word
    {
        [JsonProperty("start")]
        public long Start { get; internal set; }

        [JsonProperty("end")]
        public long End { get; internal set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; internal set; }

        [JsonProperty("text")]
        public string? Text { get; internal set; }
    }
}
