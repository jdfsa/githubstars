namespace Api.Model
{
    /// <summary>
    /// Model to wrap the claims to be stored in the token
    /// </summary>
    public class TokenClaims
    {
        /// <summary>
        /// Token supplied from GitHub
        /// </summary>
        public string GitHubToken { get; set; }

        /// <summary>
        /// Logged user data
        /// </summary>
        public User User { get; set; }
    }
}
