using Microsoft.Extensions.Configuration;
using Moq;

namespace Api.Test.Helper
{
    /// <summary>
    /// Configuration mock generator
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Returns a mocked instance of IConfiguration
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfigurationMock()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(g => g["GitHubData:ClientId"]).Returns("_client_id_test");
            configuration.Setup(g => g["GitHubData:ClientSecret"]).Returns("_client_secret_test");
            configuration.Setup(g => g["GitHubData:Scope"]).Returns("_scope:t1,scope:t2");
            configuration.Setup(g => g["GitHubEndpoints:Authorize"]).Returns("http://url-test.com/authorize");
            configuration.Setup(g => g["GitHubEndpoints:AccessToken"]).Returns("http://url-test.com/access_token");
            configuration.Setup(g => g["GitHubEndpoints:GraphQL"]).Returns("http://url-test.com/graphql");
            return configuration.Object;
        }
    }
}
