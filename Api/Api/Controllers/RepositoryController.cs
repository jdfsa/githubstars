using Api.Service;
using GraphQL.Client;
using GraphQL.Client.Exceptions;
using GraphQL.Common.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/repository")]
    public class RepositoryController : BaseController, IDisposable
    {
        /// <summary>
        /// Configuration data from appsettings.json
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// GraphQL client to retrieve data from GitHub
        /// </summary>
        private GraphQLService service;

        /// <summary>
        /// App configuration with data from appsettings.json
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        public RepositoryController(IConfiguration configuration)
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
        /// Retrieves a user from GitHub based on search criteria
        /// </summary>
        /// <param name="token">Authorization token required from GitHub</param>
        /// <param name="search">Search criteria (user name or nickname)</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(
            [FromHeader(Name = "Authorization")] string token, 
            [FromQuery] string search)
        {
            try
            {
                service.Headers.Add("Authorization", token);
                service.Headers.Add("User-Agent", token);

                GraphQLRequest request = new GraphQLRequest
                {
                    Query = @" 
                        query { 
                            search(query: ""#username#"", type: USER, first: 1) {
                                edges {
                                    node {
                                        ... on User {
                                            id,
                                            avatarUrl,
                                            name,
                                            login,
                                            bio,
                                            company,
                                            companyHTML,
                                            location,
                                            email,
                                            websiteUrl,
                                            repositories(first: 4) {
                                                edges {
                                                    node {
                                                        id,
                                                        name,
                                                        description,
                                                        stargazers {
                                                            totalCount
                                                        }
                                                        viewerHasStarred
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }"
                };
                request.Query = request.Query.Replace("#username#", search);

                var result = service.PostData(request).Result;
                return StatusCode((int)HttpStatusCode.OK, result);
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