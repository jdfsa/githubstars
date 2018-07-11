using Newtonsoft.Json;

namespace Api.Model
{
    /// <summary>
    /// Access token response model
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Access token suplieed from GitHub
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Access token type
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Scope permissions
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Error identification if it occurs
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Error description if it occurs
        /// </summary>
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
