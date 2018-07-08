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
    public class RepositoryControllerTest
    {
        RepositoryController controller;

        public RepositoryControllerTest()
        {
            var configuration = ConfigurationHelper.GetConfigurationMock();
            var service = new GraphQLServiceFake("http://fakeendpoint.com/fake")
            {
                Response = new GraphQLResponse
                {
                    Data = JsonConvert.DeserializeObject("{\"search\":{\"edges\":[{\"node\":{\"id\":\"MDQ6VXNlcjE0MTAxMDY=\",\"avatarUrl\":\"https://avatars2.githubusercontent.com/u/1410106?v=4\",\"name\":\"Shuvalov Anton\",\"login\":\"A\",\"bio\":\"Technical Group Manager at Lazada\",\"bioHTML\":\"<div>Technical Group Manager at Lazada</div>\",\"following\":{\"edges\":[{\"node\":{\"login\":\"robbyrussell\"}},{\"node\":{\"login\":\"tpope\"}},{\"node\":{\"login\":\"kangax\"}}]},\"email\":\"anton@shuvalov.info\",\"websiteUrl\":\"http://shuvalov.info\",\"repositories\":{\"edges\":[{\"node\":{\"id\":\"MDEwOlJlcG9zaXRvcnkxNzg2NDM4\",\"name\":\"move.js\",\"description\":\"CSS3 backed JavaScript animation framework\",\"stargazers\":{\"totalCount\":4341},\"viewerHasStarred\":false}},{\"node\":{\"id\":\"MDEwOlJlcG9zaXRvcnk5NTA1OTk5\",\"name\":\"grunt-ect\",\"description\":\"generates multiple html files from ect templates\",\"stargazers\":{\"totalCount\":8},\"viewerHasStarred\":false}},{\"node\":{\"id\":\"MDEwOlJlcG9zaXRvcnkxNDY2NzA0MQ==\",\"name\":\"simple-grid-stylus\",\"description\":\"Simple fixed customizable grid inspired bootstrap\",\"stargazers\":{\"totalCount\":0},\"viewerHasStarred\":false}},{\"node\":{\"id\":\"MDEwOlJlcG9zaXRvcnkxNDcxOTU1OQ==\",\"name\":\"shuvalov.info\",\"description\":\"My blog \",\"stargazers\":{\"totalCount\":1},\"viewerHasStarred\":false}}]}}}]}}")
                }
            };
            var authControllerMock = new Mock<UserController>();
            this.controller = new RepositoryController(configuration);
            this.controller.Initialize(configuration, service);
        }

        [Fact]
        public void GetTest()
        {
            var expected = new UserInfo
            {
                Id = "MDQ6VXNlcjE0MTAxMDY=",
                AvatarUrl = "https=//avatars2.githubusercontent.com/u/1410106?v=4",
                Name = "Shuvalov Anton",
                Login = "A",
                Bio = "Technical Group Manager at Lazada",
                Email = "anton@shuvalov.info",
                WebSiteUrl = "http://shuvalov.info",
                Repositories = new List<Repository>()
                {
                    new Repository {
                       Id="MDEwOlJlcG9zaXRvcnkxNzg2NDM4",
                       Name="move.js",
                       Description="CSS3 backed JavaScript animation framework",
                       StargazesCount=4341,
                       ViewerHasStarred=false
                    },
                    new Repository {
                       Id="MDEwOlJlcG9zaXRvcnk5NTA1OTk5",
                       Name="grunt - ect",
                       Description="generates multiple html files from ect templates",
                       StargazesCount=8,
                       ViewerHasStarred=false
                    },
                    new Repository{
                       Id="MDEwOlJlcG9zaXRvcnkxNDY2NzA0MQ==",
                       Name="simple-grid-stylus",
                       Description="Simple fixed customizable grid inspired bootstrap",
                       StargazesCount = 0,
                       ViewerHasStarred=false
                    },
                    new Repository{
                       Id="MDEwOlJlcG9zaXRvcnkxNDcxOTU1OQ==",
                       Name="shuvalov.info",
                       Description="My blog ",
                       StargazesCount=1,
                       ViewerHasStarred=false
                    }
                }
            };
            var result = (ObjectResult)controller.Get("tokentest", string.Empty);
            var actual = result.Value as UserInfo;

            Assert.Equal(200, result.StatusCode.GetValueOrDefault(0));
            Assert.True(expected.EquivalentTo(actual));
        }

        [Fact]
        public void GetExceptionTest()
        {
            var result = (ObjectResult)controller.Get(string.Empty, string.Empty);

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
