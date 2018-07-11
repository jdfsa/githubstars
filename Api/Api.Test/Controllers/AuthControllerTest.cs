using Api.Controllers;
using Api.Model;
using Api.Test.Fakes;
using Api.Test.Helper;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Api.Test.Controllers
{
    public class AuthControllerTest
    {
        private IConfiguration configuration;
        private AuthController controller;

        public AuthControllerTest()
        {
            this.configuration = ConfigurationHelper.GetConfigurationMock();
            this.controller = new AuthController(configuration);
        }

        [Fact]
        public void TestGetUrl()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(x => x.Next()).Returns(99887766);

            this.controller.Initialize(configuration, randomMock.Object, null, null);
            var expectedUrl = "http://url-test.com/authorize?client_id=_client_id_test&scope=_scope:t1,scope:t2&redirect_uri=http://url-callback-teste.com&state=R2l0SHViU3RhcnM5OTg4Nzc2Ng==";
            var actualUrl = controller.GetUrl("http://url-callback-teste.com");

            Assert.Equal(expectedUrl, actualUrl);
        }

        [Fact]
        public void TestGetToken()
        {
            var httpClient = new HttpClient(new HttpHandlerFake(
                HttpStatusCode.OK,
                "{\"access_token\":\"access_token_test\",\"token_type\":\"bearer\",\"scope\":\"scope_test\"}"));

            var graphService = new GraphQLServiceFake("http://fakeendpoint.com/fake")
            {
                Response = new GraphQLResponse
                {
                    Data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                    {
                        viewer = new { ID = "idtest" }
                    }))
                }
            };

            this.controller.Initialize(configuration, null, httpClient, graphService);

            var result = (ObjectResult)controller.GetToken("githubcodetest", "statetest");
            var expected = new AccessTokenResponse
            {
                AccessToken = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJHaXRIdWJUb2tlbiI6ImJlYXJlciBhY2Nlc3NfdG9rZW5fdGVzdCIsIlVzZXIiOnsiaWQiOm51bGwsImF2YXRhclVybCI6bnVsbCwibmFtZSI6bnVsbCwibG9naW4iOm51bGx9fQ",
                TokenType = "bearer",
                Scope = "scope_test",
                Error = null,
                ErrorDescription = null
            };

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.True(expected.EquivalentTo(result.Value));
        }
    }
}

