using System;
using System.Net;

namespace Api.Model
{
    /// <summary>
    /// HttpException model
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// Status code which refers to the exception
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">Status code</param>
        public HttpException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
