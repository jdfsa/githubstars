using Newtonsoft.Json;

namespace Api.Model
{
    public class StarringRequest
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("repoId")]
        public string RepositoryId { get; set; }

        [JsonProperty("starring")]
        public bool Starring { get; set; }
    }
}
