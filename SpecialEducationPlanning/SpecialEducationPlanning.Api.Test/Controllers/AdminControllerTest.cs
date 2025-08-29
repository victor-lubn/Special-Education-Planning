using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Business.Model;

using Moq;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Test.Support;
using MediatR;
using Shouldly;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class AdminControllerTest : BaseTest
    {
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly Mock<IAiepRepository> _AiepRepository;
        private readonly Mock<ILogRepository> _logRepository;
        private readonly Mock<IRoleRepository> _roleRepository;
        private readonly Mock<IAzureSearchManagementService> _azureSearchManagementService;
        private readonly Mock<IFeatureManagementService> _featureManagementService;
        private readonly Mock<ILogger<AdminController>> _mockLogger;
        private readonly Mock<IMediator> _mockMediator;
        private readonly AdminController _adminController;

        public AdminControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            this._serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            this._AiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            this._logRepository = new Mock<ILogRepository>(MockBehavior.Strict);
            this._roleRepository = new Mock<IRoleRepository>(MockBehavior.Strict);
            this._azureSearchManagementService = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);
            this._featureManagementService = new Mock<IFeatureManagementService>(MockBehavior.Strict);
            this._mockLogger = new Mock<ILogger<AdminController>>();
            this._mockMediator = new Mock<IMediator>();

            this._adminController = new AdminController(
                this._serviceProvider.Object,
                this._AiepRepository.Object,
                this._logRepository.Object,
                this._roleRepository.Object,
                this._azureSearchManagementService.Object,
                this._featureManagementService.Object,
                this._mockLogger.Object,
                this._mockMediator.Object);
        }

        [Fact]
        public async void ImportZones_ThrowsNotImplementedException()
        {
            //Arrange.

            //Act.
            try
            {
                var zones = await _adminController.ImportZones();
            }
            catch (Exception e)
            {
                Assert.Equal(typeof(NotImplementedException), e.GetType());
            }
        }

        [Fact]
        public async void RefreshAiepAcl_ReturnsOkObjectResult()
        {
            //Arrange.
            _AiepRepository.Setup(rep => rep.UpdateAiepAclAsync(It.IsAny<int>())).ReturnsAsync(new RepositoryResponse<bool>());

            //Act.
            var result = await _adminController.RefreshAiepAcl(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void RefreshAllAiepAcl_ReturnsOkObjectResult()
        {
            //Arrange.
            _AiepRepository.Setup(rep => rep.UpdateAllAiepAclAsync()).ReturnsAsync(new RepositoryResponse<bool>());

            //Act.
            var result = await _adminController.RefreshAllAiepAcl();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void RefreshPermissions_ReturnsOkResult()
        {
            //Arrange.
            _roleRepository.Setup(rep => rep.RefreshPermissionListAsync())
                .ReturnsAsync(new RepositoryResponseGeneric());

            //Act.
            var result = await _adminController.RefreshPermissions();

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void GetAllLog_ReturnsOkResult()
        {
            //Arrange.
            _logRepository.Setup(rep => rep.GetAllLog(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<LogModel>>());

            //Act.
            var result = await _adminController.GetAllLog(It.IsAny<int>(), It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void EnsureAzureSearchCreated_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "Failed to ensure the Azure Search components are created...";
            _azureSearchManagementService.Setup(service => service.EnsureCreatedAsync(false))
                .ReturnsAsync(false);

            //Act.
            var result = await _adminController.EnsureAzureSearchCreated();

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal(errorString, value);
        }

        [Fact]
        public async void EnsureAzureSearchCreated_ReturnsOkResult()
        {
            //Arrange.
            _azureSearchManagementService.Setup(service => service.EnsureCreatedAsync(false))
                .ReturnsAsync(true);

            //Act.
            var result = await _adminController.EnsureAzureSearchCreated();

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void DeleteAzureSearch_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var errorString = "Failed to delete the Azure Search components...";
            _azureSearchManagementService.Setup(service => service.DeleteAzureSearch()).Returns(false);

            //Act.
            var result = await _adminController.DeleteAzureSearch();

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal(errorString, value);
        }

        [Fact]
        public async void DeleteAzureSearch_ReturnsOkResult()
        {
            //Arrange.
            _azureSearchManagementService.Setup(service => service.DeleteAzureSearch()).Returns(true);

            //Act.
            var result = await _adminController.DeleteAzureSearch();

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void HealthCheck_ReturnsOkObjectResult()
        {
            //Arrange.

            //Act.
            var result = await _adminController.HealthCheck();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        # region HealthCheckLaunchDarkly

        [Fact]
        public async void HealthCheckLaunchDarkly_SuccessfulCheck_ReturnsOk()
        {
            //Arrange
            this._mockLogger.MockLog(LogLevel.Debug);
            this._featureManagementService.Setup(x => x.HealthCheck()).ReturnsAsync(true);

            //Act
            OkResult result = await this._adminController.HealthCheckLaunchDarkly() as OkResult;

            //Assert
            this._mockLogger.VerifyLogger(Times.Exactly(2), LogLevel.Debug);
            this._featureManagementService.Verify();
            result.StatusCode.ShouldBe(200);
            this.ResetSetups();
        }

        [Fact]
        public async void HealthCheckLaunchDarkly_FailedCheck_ReturnsBadRequest()
        {
            //Arrange
            this._mockLogger.MockLog(LogLevel.Debug);
            this._mockLogger.MockLog(LogLevel.Warning);
            this._featureManagementService.Setup(x => x.HealthCheck()).ReturnsAsync(false);

            //Act
            BadRequestObjectResult result = await this._adminController.HealthCheckLaunchDarkly() as BadRequestObjectResult;

            //Assert
            this._mockLogger.VerifyLogger(Times.Once(), LogLevel.Debug);
            this._mockLogger.VerifyLogger(Times.Once(), LogLevel.Warning);
            this._featureManagementService.Verify();
            result.StatusCode.ShouldBe(400);
            this.ResetSetups();
        }

        #endregion

        #region Privates

        private void ResetSetups()
        {
            this._serviceProvider.Reset();
            this._AiepRepository.Reset();
            this._logRepository.Reset();
            this._roleRepository.Reset();
            this._azureSearchManagementService.Reset();
            this._featureManagementService.Reset();
            this._mockLogger.Reset();
        }

        #endregion
    }
}

