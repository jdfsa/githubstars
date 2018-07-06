using Newtonsoft.Json;

namespace Api.Model
{
    /// <summary>
    /// Response exception model
    /// </summary>
    public class ResponseException
    {
        /// <summary>
        /// Status code
        /// </summary>
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
