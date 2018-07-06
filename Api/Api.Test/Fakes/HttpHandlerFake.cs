using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Test.Fakes
{
    /// <summary>
    /// Handler to be used for mock purposes
    /// </summary>
    public class HttpHandlerFake : HttpMessageHandler
    {
        /// <summary>
        /// Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode">Default status code</param>
        /// <param name="content">Default content</param>
        public HttpHandlerFake(HttpStatusCode statusCode, string content)
        {
            this.StatusCode = statusCode;
            this.Content = content;
        }

        /// <summary>
        /// Override of HttpClient.SendAsync method
        /// </summary>
        /// <param name="request">Request instance</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var task = new Task<HttpResponseMessage>(() =>
            {
                var message = new HttpResponseMessage(StatusCode)
                {
                    Content = new StringContent(Content)
                };
                return message;
            });

            // starts the task so the result can me returned promptly
            task.Start();

            return task;
        }
    }
}
