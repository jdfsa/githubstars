using Newtonsoft.Json;

namespace Api.Model
{
    public class User
    {
        /// <summary>
        /// User id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// URL of user avatar
        /// </summary>
        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// User login
        /// </summary>
        [JsonProperty("login")]
        public string Login { get; set; }
    }
}
