using System.Net;
using System.Threading.Tasks;

using Koa.Hosting.AspNetCore.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.CsvFile;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

using Moq;
using Xunit;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class CsvFileControllerTest : BaseTest
    {
        private readonly string entity = "User";

        private readonly Mock<ICsvFileService> _mockFileService;

        private readonly CsvFileController csvFileController;

        public CsvFileControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockFileService = new Mock<ICsvFileService>(MockBehavior.Strict);

            csvFileController = new CsvFileController(
                this.LoggerFactory.CreateLogger<CsvFileController>(),
                _mockFileService.Object
            );
        }

        #region Private Methods
        private MultiUploadedFileModel MultipleFileInstance()
        {
            return new MultiUploadedFileModel();
        }
        #endregion

        #region Test Methods

        #region Dump CSV
        [Fact]
        public async Task UploadUserCsv_Ok()
        {
            // Arrange
            _mockFileService.Setup(fs => fs.DumpCsv(It.IsAny<string>(), It.IsAny<MultiUploadedFileModel>()))
                .ReturnsAsync(new RepositoryResponse<int>() { Content = 1 });

            // Act 
            var result = await csvFileController.UploadCsv(entity, MultipleFileInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion
    }
}
