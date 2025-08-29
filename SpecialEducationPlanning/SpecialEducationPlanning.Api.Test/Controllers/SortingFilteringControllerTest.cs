using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Api.Test.Support;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class SortingFilteringControllerTest : BaseTest
    {
        private readonly Mock<ISortingFilteringRepository> mockRepository;
        private readonly Mock<ILogger<SortingFilteringController>> mockLogger;
        private readonly SortingFilteringController controller;

        public SortingFilteringControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockRepository = new Mock<ISortingFilteringRepository>(MockBehavior.Strict);

            mockRepository.Setup(m => m.GetSortingFilteringOptionsByEntity(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<SortingFilteringModel>>() { Content = GetAll() });

            mockLogger = new Mock<ILogger<SortingFilteringController>>(MockBehavior.Default);

            controller = new SortingFilteringController(mockRepository.Object, mockLogger.Object);
        }

        private ICollection<SortingFilteringModel> GetAll()
        {
            ICollection<SortingFilteringModel> models = new List<SortingFilteringModel>
            {
                new SortingFilteringModel()
                {
                    Id = 1
                }
            };
            return models;
        }

        [Fact]
        public async Task GetSortingFilteringOptionsByEntityTest()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act
            var result = await controller.GetSortingFilteringOptionsByEntity("") as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "SortingFilteringController called GetSortingFilteringOptionsByEntity", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "SortingFilteringController end call -> return List of sorting filtering options", times);
        }
    }
}