using Api.Controllers;
using Api.Model;
using Api.Service;
using Api.Test.Fakes;
using Api.Test.Helper;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Api.Test.Controllers
{
    public class UserControllerTest
    {
        UserController controller;

        public UserControllerTest()
        {
            var configuration = ConfigurationHelper.GetConfigurationMock();
            var service = new GraphQLServiceFake("http://fakeendpoint.com/fake")
            {
                Response = new GraphQLResponse
                {
                    Data = new
                    {
                        viewer = new
                        {
                            login = "logintest",
                            avatarUrl = "https://avatarexamplle.com?id=x"
                        }
                    }
                }
            };
            var authControllerMock = new Mock<UserController>();
            this.controller = new UserController(configuration);
            this.controller.Initialize(configuration, service);
        }

        [Fact]
        public void GetUserDataTest()
        {
            var expected = new
            {
                viewer = new
                {
                    login = "logintest",
                    avatarUrl = "https://avatarexamplle.com?id=x"
                }
            };
            var result = (ObjectResult)controller.GetUserData("tokentest");
            var actual = result.Value;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetUserDataExceptionTest()
        {
            var result = (ObjectResult)controller.GetUserData(string.Empty);

            // assert status
            Assert.Equal(500, result.StatusCode.GetValueOrDefault(0));

            // assert specific type
            Assert.IsType<Dictionary<string, List<string>>>(result.Value);

            // assert specfic value
            var actual = result.Value as Dictionary<string, List<string>>;
            Assert.NotNull(actual["errors"]);
        }

    }
}
