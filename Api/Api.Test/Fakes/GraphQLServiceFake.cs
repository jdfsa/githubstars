using Api.Service;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test.Fakes
{
    public class GraphQLServiceFake : GraphQLService
    {
        public GraphQLServiceFake(string endpoint) : base(endpoint)
        {
        }

        public GraphQLResponse Response { get; set; }

        public override Task<GraphQLResponse> PostData(GraphQLRequest request)
        {
            var task = new Task<GraphQLResponse>(() =>
            {
                return Response;
            });
            task.Start();
            return task;
        }
    }
}
