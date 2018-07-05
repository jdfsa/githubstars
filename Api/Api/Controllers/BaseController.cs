using Microsoft.AspNetCore.Mvc;
using System;
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
        protected object ParseException(Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            // create a generic response data
            object response = new
            {
                error = new
                {
                    status_code = statusCode,
                    message = ex.Message
                }
            };

            return response;
        }
    }
}