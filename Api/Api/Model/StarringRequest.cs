using Newtonsoft.Json;

namespace Api.Model
{
    /// <summary>
    /// Starring model
    /// </summary>
    public class StarringRequest
    {
        /// <summary>
        /// Repository Id
        /// </summary>
        [JsonProperty("repoId")]
        public string RepositoryId { get; set; }

        /// <summary>
        /// Flag indicating whether add or remove a star
        /// </summary>
        [JsonProperty("starring")]
        public bool Starring { get; set; }
    }
}
