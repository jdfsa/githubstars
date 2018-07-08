using Api.Model;
using Api.Service;
using GraphQL.Client;
using GraphQL.Client.Exceptions;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
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
        /// <returns>Response result</returns>
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
                    OperationName = "QueryUserByName",
                    Variables = new
                    {
                        name = search
                    },
                    Query = @"query QueryUserByName($name: String!) { 
                                search(query: $name, type: USER, first: 1) { edges { node {
                                    ... on User {
                                        id, avatarUrl, name, login, bio, company, companyHTML, location, email, websiteUrl,
                                        repositories(first: 4) {
                                            edges { node {
                                                    id, name, description, viewerHasStarred,
                                                    stargazers { totalCount }
                                            }}}}}}}}"
                };

                var result = service.PostData(request).Result;
                return base.TreatResponseMessage(result);
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
        /// Adds or removes a star over a repository
        /// </summary>
        /// <param name="token">Authorization token required from GitHub</param>
        /// <param name="data">Input data consiting basicaly of user and repository ids 
        ///     and a flag indicating whether it should add to or remove a star from the repository</param>
        /// <returns>Response rsult</returns>
        [HttpPost]
        public IActionResult Starring([FromHeader(Name = "Authorization")] string token, [FromBody] StarringRequest data)
        {
            try
            {
                service.Headers.Add("Authorization", token);
                service.Headers.Add("User-Agent", token);

                GraphQLRequest request = new GraphQLRequest
                {
                    Variables = new
                    {
                        input = new {
                            starrableId = data.RepositoryId,
                            clientMutationId = data.UserId
                        }
                    },
                    Query = data.Starring ? AddStarQuery : RemoveStarQuery
                };

                var result = service.PostData(request).Result;
                return base.TreatResponseMessage(result);
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
        /// Gets a add-star query
        /// </summary>
        private string AddStarQuery
        {
            get
            {
                return @"mutation AddStarToRepo($input: AddStarInput!) {
                            addStar (input: $input) {
                                clientMutationId,
                                starrable {
                                    id,
                                    viewerHasStarred
                                }}}";
            }
        }

        /// <summary>
        /// Gets a remove-star query
        /// </summary>
        private string RemoveStarQuery
        {
            get
            {
                return @"mutation RemoveStarToRepo($input: RemoveStarInput!) {
                          removeStar (input: $input) {
                            clientMutationId,
                            starrable {
                              id,
                              viewerHasStarred
                            }}}";
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