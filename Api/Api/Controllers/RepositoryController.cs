using Api.Model;
using Api.Service;
using GraphQL.Client.Exceptions;
using GraphQL.Common.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/repository")]
    [CheckToken]
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
            [FromHeader(Name = "X-GitHub-Token")] string token, 
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
                return base.TreatResponseMessage(result, Parse);
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
        public IActionResult Starring(
            [FromHeader(Name = "X-GitHub-Token")] string token, 
            [FromHeader(Name = "X-GitHub-User-Id")] string userId,
            [FromBody] StarringRequest data)
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
                            clientMutationId = userId
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
        /// Parses a dynamic user data to a typed one
        /// </summary>
        /// <param name="data">GraphQL user data</param>
        /// <returns>Parsed view of user</returns>
        private object Parse(dynamic data)
        {
            if (data?.search?.edges == null || !(data.search.edges as JArray).HasValues)
                return null;

            var edge = data.search.edges[0];
            if (edge == null)
                return null;

            var node = edge.node;
            if (node == null)
                throw new Exception("Wrong expected data from GitHub");

            var user = new UserInfo
            {
                Id = node.id,
                AvatarUrl = node.avatarUrl,
                Name = node.name,
                Login = node.login,
                Bio = node.bio,
                Company = node.company,
                CompanyHTML = node.companyHTML,
                Location = node.location,
                Email = node.email,
                WebSiteUrl = node.websiteUrl,
                Repositories = ParseRepos(node.repositories)
            };
            return user;
        }

        /// <summary>
        /// Parses a dynamic repositories data to a typed one
        /// </summary>
        /// <param name="data">Dynamic repositories</param>
        /// <returns>List of typedd repositories</returns>
        private List<Repository> ParseRepos(dynamic data)
        {
            if (data?.edges == null || !(data.edges as JArray).HasValues)
                throw new Exception("Wrong expected data from GitHub");

            List<Repository> repositories = new List<Repository>();
            foreach (var edge in data.edges)
            {
                var node = edge.node;
                if (node == null)
                    throw new Exception("Wrong expected data from GitHub");

                repositories.Add(new Repository
                {
                    Id = node.id,
                    Name = node.name,
                    Description = node.description,
                    ViewerHasStarred = node.viewerHasStarred,
                    StargazesCount = ((int?)node.stargazers?.totalCount).GetValueOrDefault(0)
                });
            }
            return repositories;
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