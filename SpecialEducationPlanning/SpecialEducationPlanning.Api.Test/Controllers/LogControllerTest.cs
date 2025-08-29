using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Api.Model.AutomaticRemoveItems;

using Xunit;
using Moq;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class LogControllerTest : BaseTest
    {
        private Mock<ILogRepository> mockRepository;
        private readonly Mock<IOptions<AutomaticRemoveSystemConfiguration>> mockOptions;

        public LogControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockRepository = new Mock<ILogRepository>(MockBehavior.Strict);

            mockRepository.Setup(m => m.GetAllLog(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<LogModel>>() { Content = GetAll() });
            mockRepository.Setup(m => m.GetLogsFilteredAsync(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<LogModel>>() { Content = GetAll() });

            mockOptions = new Mock<IOptions<AutomaticRemoveSystemConfiguration>>(MockBehavior.Loose);

            controller = new LogController(mockRepository.Object,
                this.LoggerFactory.CreateLogger<LogController>(),
                mockOptions.Object);
        }

        private readonly LogController controller;

        private ICollection<LogModel> GetAll()
        {
            ICollection<LogModel> models = new List<LogModel>
            {
                new LogModel()
                {
                    Id = 1
                }
            };
            return models;
        }

        [Fact]
        public async Task GetAllTest()
        {
            //Act
            var result = await controller.GetAll() as OkObjectResult;
            var model = result.Value as IEnumerable<LogModel>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(GetAll().Count, model.Count());
        }

        [Fact]
        public async Task GetLogsFilteredNullTest()
        {
            var result = await controller.GetLogsFilteredAsync(null, null, null) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLogsFilteredEmptyNullTest()
        {
            var result = await controller.GetLogsFilteredAsync("", null, null) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLogsFilteredNullLevelDatesTest()
        {
            var result = await controller.GetLogsFilteredAsync(null, new DateTime(), new DateTime()) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLogsFilteredOkTest()
        {
            var result = await controller.GetLogsFilteredAsync("Warning", new DateTime(), new DateTime()) as OkObjectResult;

            Assert.NotNull(result);
        }
    }
}