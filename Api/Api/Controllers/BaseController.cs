using Api.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        protected Dictionary<string, ResponseException> ParseException(Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new Dictionary<string, ResponseException>()
            {
                {
                    "error", new ResponseException
                    {
                        StatusCode = (int)statusCode,
                        Message = ex.Message
                    }
                }
            };
        }
    }
}