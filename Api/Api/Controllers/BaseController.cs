using Api.Model;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Api.Controllers
{
    /// <summary>
    /// Base controller for reusing
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Parse an exception to an object for response purposes
        /// </summary>
        /// <param name="ex">Exception to be send</param>
        /// <param name="statusCode">Status code</param>
        /// <returns>Object with response data</returns>
        protected Dictionary<string, List<string>> ParseException(Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            // following the graphQL errors response pattern
            return new Dictionary<string, List<string>>()
            {
                {
                    "errors", new List<string>() { ex.Message }
                }
            };
        }

        /// <summary>
        /// Parses a graph QL errors response avoding technical data exposure (like error locations in the query)
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected Dictionary<string, List<string>> ParseErros(GraphQLError[] errors)
        {
            return new Dictionary<string, List<string>>()
            {
                {
                    "errors", errors.Select(x => x.Message).ToList()
                }
            };
        }

        /// <summary>
        /// Treats thr response result to the client
        /// </summary>
        /// <param name="graphResponse">GraphQL response data</param>
        /// <param name="parser">Custom parser for the result</param>
        /// <returns>Final response data</returns>
        protected IActionResult TreatResponseMessage(GraphQLResponse graphResponse, Func<GraphQLResponse, Object> parser = null)
        {
            // treats errors as band request (400)
            if (graphResponse.Errors != null && graphResponse.Errors.Length > 0)
                return StatusCode((int)HttpStatusCode.BadRequest, ParseErros(graphResponse.Errors));

            // treats no data at all as unauthorized
            if (graphResponse.Data == null)
                return StatusCode((int)HttpStatusCode.Unauthorized, new { });

            // if there is a parser function, returns its response data; otherwise returns the original graph response
            object response = parser != null ? parser.Invoke(graphResponse) : graphResponse.Data;

            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}