using Api.Model;
using Api.Service;
using GraphQL.Client.Exceptions;
using GraphQL.Common.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    [CheckToken]
    public class UserController : BaseController, IDisposable
    {
        /// <summary>
        /// Configuration data from appsettings.json
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// GraphQL service to retrieve data from GitHub
        /// </summary>
        private GraphQLService service;

        /// <summary>
        /// App configuration with data from appsettings.json
        /// </summary>
        /// <param name="configuration">Injected configuration instance</param>
        public UserController(IConfiguration configuration)
        {
            this.Initialize(configuration, GraphQLService.GetInstance(configuration["GitHubEndpoints:GraphQL"]));
        }

        /// <summary>
        /// Initializes the controller
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="service">GraphQL service</param>
        public void Initialize(IConfiguration configuration, GraphQLService service)
        {
            this.configuration = configuration;
            this.service = service;
        }

        /// <summary>
        /// Gets user data
        /// </summary>
        /// <param name="token">Authorization token required from GitHub</param>
        /// <returns>Response data</returns>
        [HttpGet]
        public IActionResult GetUserData([FromHeader(Name = "X-GitHub-Token")] string token)
        {
            try
            {
                service.Headers.Clear();
                service.Headers.Add("Authorization", token);
                service.Headers.Add("User-Agent", token);

                GraphQLRequest request = new GraphQLRequest
                {
                    Query = @"query { viewer { id, login, avatarUrl, name } }"
                };

                var result = service.PostData(request).Result;
                if (result.Data == null && result.Errors == null)
                    return StatusCode((int)HttpStatusCode.Unauthorized, new { });

                return base.TreatResponseMessage(result, resp =>
                {
                    if (resp == null || resp.viewer == null)
                        throw new Exception("Required data (viewer) not supplied");

                    return new User
                    {
                        Id = resp.viewer.id,
                        Login = resp.viewer.login,
                        AvatarUrl = resp.viewer.avatarUrl,
                        Name = resp.viewer.name
                    };
                });
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

        /// <summary>
        /// Disposing controller
        /// </summary>
        /// <param name="disposing">True if invoked by the Microsoft.AspNetCore.Mvc.Controller.Dispose method</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}