using Api.Controllers;
using Api.Test.Fakes;
using Api.Test.Helper;
using Microsoft.AspNetCore.Mvc;
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
        AuthController controller;

        public AuthControllerTest()
        {
            var configuration = ConfigurationHelper.GetConfigurationMock();

            var random = new Mock<Random>();
            random.Setup(x => x.Next()).Returns(99887766);

            var httpClient = new HttpClient(new HttpHandlerFake(
                HttpStatusCode.OK,
                "{\"access_token\":\"access_token_test\",\"token_type\":\"bearer\",\"scope\":\"scope_test\"}"));

            this.controller = new AuthController(configuration);
            this.controller.Initialize(configuration, random.Object, httpClient);
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

