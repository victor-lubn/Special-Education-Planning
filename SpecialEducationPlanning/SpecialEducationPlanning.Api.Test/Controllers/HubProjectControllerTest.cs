using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.ProjectService;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Repository;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class HubProjectControllerTest : BaseTest
    {
        public HubProjectControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var mockService = new Mock<IProjectService>(MockBehavior.Strict);
            mockService.Setup(m => m.CreateUpdateProjectForCreatio(It.IsAny<CreatioProjectDto>()))
                .ReturnsAsync((CreatioProjectDto creatio) =>
                {
                    if (string.IsNullOrWhiteSpace(creatio.CodeProject))
                    {
                        return new RepositoryResponse<string>(new List<string>() { "error"});
                    }
                    return new RepositoryResponse<string>("keyname", new List<string>());
                });

            controller = new HubProjectController(
                this.LoggerFactory.CreateLogger<HubProjectController>(),
                mockService.Object);
        }

        public readonly HubProjectController controller;

        [Fact]
        public async Task PostTest()
        {
            //Arrange
            var apiVersion = "version";
            var appName = "app";
            var creatio = new CreatioProjectDto() { CodeProject = "Test" };

            //Act
            var result = await controller.Post(apiVersion, appName, creatio) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("keyname", result.Value);
        }

        [Fact]
        public async Task PostBadRequestTest()
        {
            //Arrange
            var apiVersion = "version";
            var appName = "app";
            var creatio = new CreatioProjectDto() { CodeProject = "  " };

            //Act
            var result = await controller.Post(apiVersion, appName, creatio) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("error", ((List<string>)result.Value).First());
        }

    }
}
