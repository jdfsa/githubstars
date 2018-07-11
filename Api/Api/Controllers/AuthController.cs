using Api.Model;
using Api.Service;
using GraphQL.Common.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Api.Controllers
{
    /// <summary>
    /// Handler for authentication process
    /// </summary>
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        /// <summary>
        /// App configuration with data from appsettings.json
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// GraphQL service to retrieve data from GitHub
        /// </summary>
        private GraphQLService service;

        /// <summary>
        /// Random values
        /// </summary>
        private Random random;

        /// <summary>
        /// Client for remote requests
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        public AuthController(IConfiguration configuration)
        {
            Initialize(configuration, new Random(), new HttpClient(), GraphQLService.GetInstance(configuration["GitHubEndpoints:GraphQL"]));
        }

        /// <summary>
        /// Initialize with all used instances
        /// </summary>
        /// <param name="cnfiguration">Configuration object</param>
        /// <param name="random">Random instance</param>
        /// <param name="httpClient">httpClient instance</param>
        /// <param name="service">httpClient instance</param>
        public void Initialize(IConfiguration cnfiguration, Random random, HttpClient httpClient, GraphQLService service)
        {
            this.configuration = cnfiguration;
            this.random = random;
            this.httpClient = httpClient;
            this.service = service;
        }

        /// <summary>
        /// Returns the GitHub login page address
        /// </summary>
        /// <param name="urlBack">URL to be redirected after login</param>
        /// <returns>URL generated</returns>
        [HttpGet]
        [Route("url")]
        public string GetUrl([FromQuery] string urlBack)
        {
            try
            {
                string passCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    string.Format("GitHubStars{0}", random.Next())));

                // return the querystring formatted
                return string.Format("{0}?client_id={1}&scope={2}&redirect_uri={3}&state={4}",
                    configuration["GitHubEndpoints:Authorize"],
                    configuration["GitHubData:ClientId"],
                    configuration["GitHubData:Scope"],
                    urlBack, passCode);
            }
            catch (FormatException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return ex.Message;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return ex.Message;
            }
        }

        /// <summary>
        /// Get token from GitHub based on the data generated previously
        /// </summary>
        /// <param name="githubCode">GitHub code given to the user</param>
        /// <param name="state">The same "state" data generated previously from the GetUrl in this API</param>
        /// <returns>Action result with the response data and status code</returns>
        [HttpGet]
        [Route("token")]
        public IActionResult GetToken([FromQuery] string githubCode, [FromQuery] string state)
        {
            try
            {
                // http client for the request
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // request objetct
                AccessTokenRequest request = new AccessTokenRequest
                {
                    ClientId = configuration["GitHubData:ClientId"],
                    ClientSecret = configuration["GitHubData:ClientSecret"],
                    Code = githubCode,
                    State = state
                };
                string requestJson = JsonConvert.SerializeObject(request);

                // make the call and waits for the response
                var response = httpClient.PostAsync(
                    configuration["GitHubEndpoints:AccessToken"],
                    new StringContent(requestJson, Encoding.UTF8, "application/json")).Result;

                // check status code
                if ((int)response.StatusCode >= 400)
                    return StatusCode((int)response.StatusCode);

                // read and return the response payload
                string content = response.Content.ReadAsStringAsync().Result;

                // parse the object taken
                var gitResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(content);

                // returns forbidden if an error occured, so the user must authenticate again
                if (!string.IsNullOrEmpty(gitResponse.Error))
                    throw new HttpException(gitResponse.ErrorDescription, HttpStatusCode.Forbidden);

                string gitHubToken = string.Format("{0} {1}", gitResponse.TokenType, gitResponse.AccessToken);
                var user = ValidateGithubToken(gitHubToken);
                var finalToken = GenerateApptoken(gitHubToken, user);

                // overrides the response token and send to the user
                gitResponse.AccessToken = finalToken;
                return StatusCode(200, gitResponse);
            }
            catch (HttpException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (JsonSerializationException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ParseException(ex));
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ParseException(ex));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ParseException(ex));
            }
        }

        /// <summary>
        /// Validates the GitHub token by getting the logged user
        /// </summary>
        /// <param name="token">GitHub token</param>
        /// <returns>User data</returns>
        private User ValidateGithubToken(string token)
        {
            service.Headers.Clear();
            service.Headers.Add("Authorization", token);
            service.Headers.Add("User-Agent", token);

            GraphQLRequest request = new GraphQLRequest
            {
                Query = @"query { viewer { id } }"
            };

            var result = service.PostData(request).Result;
            if (result.Errors != null || result.Data == null || result.Data.viewer == null)
                throw new HttpException("No user found with the given token", HttpStatusCode.Forbidden);

            return new User
            {
                Id = result.Data.viewer.id
            };
        }

        /// <summary>
        /// Generates the token
        /// </summary>
        /// <param name="token">GitHub token</param>
        /// <param name="user">User data to be wrapped in the token</param>
        /// <returns>Generated token</returns>
        private string GenerateApptoken(string token, User user)
        {
            TokenClaims tokenSession = new TokenClaims
            {
                GitHubToken = token,
                User = user
            };
            return Jose.JWT.Encode(tokenSession, configuration["TokenKey"], Jose.JwsAlgorithm.none);
        }

        /// <summary>
        /// Options verb
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        [Route("url")]
        [Route("token")]
        public string Options()
        {
            return "GET,POST";
        }
    }
}