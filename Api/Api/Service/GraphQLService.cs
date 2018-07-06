using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Service
{
    /// <summary>
    /// Service to post data to GitHub
    /// </summary>
    public abstract class GraphQLService : IDisposable
    {
        /// <summary>
        /// The GraphQLClient
        /// </summary>
        public GraphQLClient Client { get; private set; }

        /// <summary>
        /// Request headers
        /// </summary>
        public virtual HttpRequestHeaders Headers
        {
            get
            {
                return this.Client.DefaultRequestHeaders;
            }
        }

        /// <summary>
        /// Builds the service with an endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint to be used in the GraphQLClient</param>
        public GraphQLService(string endpoint) : this(new GraphQLClient(endpoint)) { }

        /// <summary>
        /// Builds the service with a GraphQLClient
        /// </summary>
        /// <param name="graphQLClient">Instance of GraphQLClient</param>
        public GraphQLService(GraphQLClient graphQLClient)
        {
            Client = graphQLClient;
        }

        /// <summary>
        /// Dispose instance
        /// </summary>
        public void Dispose()
        {
            this.Client.Dispose();
        }

        /// <summary>
        /// Post data to GitHub
        /// </summary>
        /// <param name="request">Request message</param>
        /// <returns>Task with the response</returns>
        public abstract Task<GraphQLResponse> PostData(GraphQLRequest request);

        /// <summary>
        /// Get a real implementation of GraphQLService
        /// </summary>
        /// <param name="graphQLClient"></param>
        /// <returns>Instance of GraphQLService</returns>
        public static GraphQLService GetInstance(string endpoint)
        {
            return new GraphQLServiceImpl(endpoint);
        }

        /// <summary>
        /// Get a real implementation of GraphQLService
        /// </summary>
        /// <param name="graphQLClient"></param>
        /// <returns>Instance of GraphQLService</returns>
        public static GraphQLService GetInstance(GraphQLClient graphQLClient)
        {
            return new GraphQLServiceImpl(graphQLClient);
        }

        /// <summary>
        /// Real implementation of GraphQLService
        /// </summary>
        private class GraphQLServiceImpl : GraphQLService
        {
            /// <summary>
            /// Builds the service with an endpoint
            /// </summary>
            /// <param name="endpoint">The endpoint to be used in the GraphQLClient</param>
            public GraphQLServiceImpl(string endpoint) : base(new GraphQLClient(endpoint)) { }

            /// <summary>
            /// Builds the service with a GraphQLClient
            /// </summary>
            /// <param name="graphQLClient">Instance of GraphQLClient</param>
            public GraphQLServiceImpl(GraphQLClient graphQLClient) : base(graphQLClient) { }

            /// <summary>
            /// Post data to GitHub
            /// </summary>
            /// <param name="request">Request message</param>
            /// <returns>Task with the response</returns>
            public override Task<GraphQLResponse> PostData(GraphQLRequest request)
            {
                return Client.PostAsync(request);
            }
        }
    }
}
