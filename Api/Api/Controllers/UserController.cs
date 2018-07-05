using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Client.Exceptions;
using GraphQL.Common.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserController : BaseController
    {
        IConfiguration configuration;

        /// <summary>
        /// App configuration with data from appsettings.json
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets user data
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>Response data</returns>
        [HttpGet]
        public IActionResult GetUserData([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                using (GraphQLClient client = new GraphQLClient(configuration["GitHubEndpoints:GraphQL"]))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    client.DefaultRequestHeaders.Add("User-Agent", token);

                    GraphQLRequest request = new GraphQLRequest
                    {
                        Query = @"query { viewer { login, avatarUrl } }"
                    };

                    return StatusCode((int)HttpStatusCode.OK, client.PostAsync(request).Result);
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is GraphQLHttpException)
                {
                    var message = (ex.InnerException as GraphQLHttpException).HttpResponseMessage;
                    return StatusCode((int)message.StatusCode, message.ReasonPhrase);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError, ParseException(ex));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ParseException(ex));
            }
        }
    }
}