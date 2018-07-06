﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.Service;
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
        public IActionResult GetUserData([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                service.Headers.Clear();
                service.Headers.Add("Authorization", token);
                service.Headers.Add("User-Agent", token);

                GraphQLRequest request = new GraphQLRequest
                {
                    Query = @"query { viewer { login, avatarUrl } }"
                };

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