using Api.Controllers;
using Api.Test.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Api.Test
{
    public class AuthControllerTest
    {
        AuthController controller;

        public AuthControllerTest()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(x => x["GitHubData:ClientId"]).Returns("_client_id_test");
            configuration.Setup(x => x["GitHubData:ClientSecret"]).Returns("_client_secret_test");
            configuration.Setup(x => x["GitHubData:Scope"]).Returns("_scope:t1,scope:t2");
            configuration.Setup(x => x["GitHubEndpoints:Authorize"]).Returns("http://url-test.com/authorize");
            configuration.Setup(x => x["GitHubEndpoints:AccessToken"]).Returns("http://url-test.com/access_token");
            configuration.Setup(x => x["GitHubEndpoints:GraphQL"]).Returns("http://url-test.com/graphql");

            var random = new Mock<Random>();
            random.Setup(x => x.Next()).Returns(99887766);

            var httpClient = new HttpClient(new HttpHandlerMock(
                HttpStatusCode.OK,
                "{\"access_token\":\"access_token_test\",\"token_type\":\"bearer\",\"scope\":\"scope_test\"}"));

            var authControllerMock = new Mock<AuthController>();
            this.controller = new AuthController(configuration.Object);
            this.controller.Initialize(configuration.Object, random.Object, httpClient);
        }

        [Fact]
        public void TestGetUrl()
        {
            var expectedUrl = "http://url-test.com/authorize?client_id=_client_id_test&scope=_scope:t1,scope:t2&redirect_uri=http://url-callback-teste.com&state=R2l0SHViU3RhcnM5OTg4Nzc2Ng==";
            var actualUrl = controller.GetUrl("http://url-callback-teste.com");

            Assert.Equal(expectedUrl, actualUrl);
        }

        [Fact]
        public void TestGetToken()
        {
            var result = (ObjectResult)controller.GetToken("githubcodetest", "statetest");
            var expected = JsonConvert.DeserializeObject("{\"access_token\":\"access_token_test\",\"token_type\":\"bearer\",\"scope\":\"scope_test\"}");

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expected, result.Value);
        }
    }
}

