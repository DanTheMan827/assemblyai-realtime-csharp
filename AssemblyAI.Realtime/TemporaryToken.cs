using Newtonsoft.Json;

namespace AssemblyAI.Realtime
{
    public class TemporaryToken
    {
        /// <summary>
        /// A temporary token to use with the realtime API.
        /// </summary>
        [JsonProperty("token")]
        public string? Token { get; internal set; }

        /// <summary>
        /// When the token expires.
        /// </summary>
        [JsonIgnore]
        public DateTime ExpiresAt { get; internal set; }

        /// <summary>
        /// Gets a temporary token with the specified expiration time.
        /// </summary>
        /// <param name="expiresIn">The expiration time for the token, in seconds. Valid values are in the range [60,360000] inclusive.</param>
        /// <returns>The generated token.</returns>
        public static async Task<TemporaryToken?> GetTemporaryTokenAsync(string authorization, int expiresIn = 360000)
        {
            // Check our authorization
            if (string.IsNullOrWhiteSpace(authorization))
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            // Check the expiresIn argument
            if (expiresIn < 60 || expiresIn > 360000)
            {
                throw new ArgumentOutOfRangeException(nameof(expiresIn), "Valid values are in the range [60,360000] inclusive.");
            }

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("authorization", authorization);

            try
            {
                // Prepare the request data
                var requestData = new { expires_in = expiresIn };
                var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await client.PostAsync("https://api.assemblyai.com/v2/realtime/token", requestContent);
                response.EnsureSuccessStatusCode();

                // Read the response and extract the token
                var responseContent = await response.Content.ReadAsStringAsync();

                var deserializedResponse = JsonConvert.DeserializeObject<TemporaryToken>(responseContent);

                if (deserializedResponse != null)
                {
                    deserializedResponse.ExpiresAt = DateTime.Now.AddSeconds(expiresIn);

                    return deserializedResponse;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error occurred while generating token.", ex);
            }
        }
    }
}
