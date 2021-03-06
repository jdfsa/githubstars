using Api.Controllers;
using Api.Model;
using Api.Test.Fakes;
using Api.Test.Helper;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                    Data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                    {
                        viewer = new
                        {
                            ID = "idtest",
                            login = "logintest",
                            avatarUrl = "https://avatarexamplle.com?id=x"
                        }
                    }))
                }
            };
            var authControllerMock = new Mock<UserController>();
            this.controller = new UserController(configuration);
            this.controller.Initialize(configuration, service);
        }

        [Fact]
        public void GetUserDataTest()
        {
            var expected = new User
            {
                Login = "logintest",
                AvatarUrl = "https://avatarexamplle.com?id=x"
            };
                
            var result = (ObjectResult)controller.GetUserData("tokentest");
            var actual = result.Value as User;

            Assert.True(expected.EquivalentTo(actual));
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
