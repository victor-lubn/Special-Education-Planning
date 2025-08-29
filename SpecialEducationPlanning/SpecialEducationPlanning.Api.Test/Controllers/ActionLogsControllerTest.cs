using System;
using System.Text;
using System.Collections.Generic;

using Koa.Persistence.Abstractions.QueryResult;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model.View;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Api.Model.AutomaticRemoveItems;

using Moq;
using Xunit;
using Xunit.Abstractions;
using Koa.Domain.Search.Page;
using Microsoft.AspNetCore.Http;
using Koa.Serialization.Json;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class ActionLogsControllerTest : BaseTest
    {
        private readonly DateTime startDate = new DateTime(2019, 8, 31);
        private readonly DateTime endDate = new DateTime(2019, 8, 31);

        private readonly int existingActionId = 1;

        private readonly Mock<IActionLogsRepository> mockActionRepository;
        private readonly Mock<IOptions<AutomaticRemoveActionConfiguration>> mockAutomaticRemoveActions;

        private readonly Mock<IServiceProvider> serviceProviderMock;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> httpContextMock;

        private readonly ActionLogsController mockController;

        public ActionLogsControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockActionRepository = new Mock<IActionLogsRepository>(MockBehavior.Strict);
            mockAutomaticRemoveActions = new Mock<IOptions<AutomaticRemoveActionConfiguration>>(MockBehavior.Loose);

            this.serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            this.httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>(MockBehavior.Strict);
            this.httpContextMock.SetupGet(context => context.RequestServices).Returns(this.serviceProviderMock.Object);

            mockController = new ActionLogsController(
                mockActionRepository.Object,
                this.LoggerFactory.CreateLogger<ActionLogsController>(),
                mockAutomaticRemoveActions.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = this.httpContextMock.Object }
            };
        }

        #region Private Methods
        private ActionLogsModel ActionExistingInstance()
        {
            var model = new ActionLogsModel
            {
                Id = existingActionId
            };
            return model;
        }

        private ICollection<ActionLogsModel> ActionListInstance()
        {
            ICollection<ActionLogsModel> areas = new List<ActionLogsModel>
            {
                ActionExistingInstance()
            };
            return areas;
        }
        #endregion

        #region Test Methods

        #region Get

        #region Get ActionLogs Filtered
        [Fact]
        public async void GetActionsLogsFiltered_ModelError_BadRequest()
        {
            // Arrange
            mockController.ModelState.AddModelError("id", "id is null");

            // Act 
            var result = await mockController.GetActionLogsFiltered(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetActionLogsFiltered_RepositoryError_BadRequest()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 100,
                Skip = 0
            };

            mockActionRepository.Setup(x => x.GetActionLogsFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<ActionLogsModel>>()
                {
                    ErrorList = new List<string>() { Domain.Enum.ErrorCode.EntityNotFound.ToString() }
                });

            // Act
            var result = await mockController.GetActionLogsFiltered(searchModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetActionLogsFiltered_DefaultTake_Ok()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 100,
                Skip = 0,
            };
            var jsonSerializerMock = new Mock<IJsonSerializer>(MockBehavior.Strict);
            this.serviceProviderMock
                .Setup(provider => provider.GetService(typeof(IJsonSerializer)))
                .Returns(jsonSerializerMock.Object);

            mockActionRepository.Setup(x => x.GetActionLogsFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<ActionLogsModel>>()
                {
                    Content = new PagedQueryResult<ActionLogsModel>(
                        ActionListInstance(),
                        searchModel.Take,
                        searchModel.Skip,
                        10
                    )
                });

            // Act
            var result = await mockController.GetActionLogsFiltered(searchModel);

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Get ActionLogs Csv
        [Fact]
        public async void GetActionLogsCSV_NoActions_NoContent()
        {
            // Arrange
            mockActionRepository.Setup(x => x.GetActionLogsCsv(startDate, endDate))
                .ReturnsAsync(new RepositoryResponse<StringBuilder>()
                {
                    Content = new StringBuilder()
                    {
                        Length = 0
                    }
                });

            // Act
            var result = await mockController.GetActionLogsCsv(startDate.ToString(), endDate.ToString());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void GetActionLogsCSV_Success_FileContent()
        {
            // Arrange
            mockActionRepository.Setup(x => x.GetActionLogsCsv(startDate, endDate))
                .ReturnsAsync(new RepositoryResponse<StringBuilder>()
                {
                    Content = new StringBuilder()
                    {
                        Length = 100
                    }
                });

            // Act
            var result = await mockController.GetActionLogsCsv(startDate.ToString(), endDate.ToString());

            // Assert
            Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", (result as FileContentResult).ContentType);
            Assert.Equal("actionLogs.csv", (result as FileContentResult).FileDownloadName);
        }
        #endregion

        #endregion

        #endregion
    }
}
