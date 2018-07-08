using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api.Model
{
    /// <summary>
    /// User model
    /// </summary>
    public class UserInfo : User
    {
        /// <summary>
        /// User bio
        /// </summary>
        [JsonProperty("bio")]
        public string Bio { get; set; }

        /// <summary>
        /// user company
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>
        /// User company formatted as HTML
        /// </summary>
        [JsonProperty("companyHTML")]
        public string CompanyHTML { get; set; }

        /// <summary>
        /// User location
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// User website url
        /// </summary>
        [JsonProperty("webSiteUrl")]
        public string WebSiteUrl { get; set; }

        /// <summary>
        /// user repositories
        /// </summary>
        [JsonProperty("repositories")]
        public List<Repository> Repositories { get; set; }
    }
}
