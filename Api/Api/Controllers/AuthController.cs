using Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Api.Controllers
{
    /// <summary>
    /// Handler for authentication process
    /// </summary>
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        /// <summary>
        /// App configuration with data from appsettings.json
        /// </summary>
        private IConfiguration configuration;

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
            Initialize(configuration, new Random(), new HttpClient());
        }

        /// <summary>
        /// Initialize with all used instances
        /// </summary>
        /// <param name="cnfiguration">Configuration object</param>
        /// <param name="random">Random instance</param>
        /// <param name="httpClient">httpClient instance</param>
        public void Initialize(IConfiguration cnfiguration, Random random, HttpClient httpClient)
        {
            this.configuration = cnfiguration;
            this.random = random;
            this.httpClient = httpClient;
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
            string passCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                string.Format("GitHubStars{0}", random.Next())));

            // return the querystring formatted
            return string.Format("{0}?client_id={1}&redirect_uri={2}&state={3}",
                configuration["GitHubEndpoints:Authorize"],
                configuration["GitHubData:ClientId"],
                urlBack, passCode);
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
                return StatusCode(200, JsonConvert.DeserializeObject(content));
            }
            catch (JsonSerializationException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
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