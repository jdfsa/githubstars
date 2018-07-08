using Newtonsoft.Json;

namespace Api.Model
{
    /// <summary>
    /// Repository model
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Repository id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Repository Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Repository description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// How many stars the repository received
        /// </summary>
        [JsonProperty("stargazers")]
        public int StargazesCount { get; set; }

        /// <summary>
        /// Indicates whether the viewer (logged user) has starred the repository
        /// </summary>
        [JsonProperty("viewerHasStarred")]
        public bool ViewerHasStarred { get; set; }
    }
}
