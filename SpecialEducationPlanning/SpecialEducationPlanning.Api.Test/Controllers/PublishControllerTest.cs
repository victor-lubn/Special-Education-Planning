using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Koa.Persistence.EntityRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SpecialEducationPlanning
.Api.Configuration.Strategy;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.Publish;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Api.Test.Support;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class PublishControllerTest : BaseTest
    {
        private readonly PublishController controller;
        const int existingPlanId = 2;
        const int existingVersionId = 7;
        const int nonExistingId = 100;
        private Mock<IPublishService> mockPublishService;
        private Mock<IFeatureManagementService> mockFeatureManagementService;
        private Mock<IPlanRepository> planMock;
        private Mock<IVersionRepository> versionMock;
        private Mock<IActionRepository> actionRepoMock;
        private Mock<IBuilderRepository> builderRepoMock;
        private Mock<IFileStorageService<AzureStorageConfiguration>> fileStorageServiceMock;
        private Mock<IUserService> mockUserService;
        private Mock<IUserRepository> mockUserRepo;
        private Mock<CountryConfiguration> mockCountryConfiguration;
        private readonly Mock<ILogger<PublishController>> mockLogger;
        private Mock<IPublishProjectService> mockPublishProjectService;
        private Mock<IPublish3DcService> mockPublish3DcService;

        public PublishControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            this.mockPublishService = new Mock<IPublishService>(MockBehavior.Strict);
            this.mockFeatureManagementService = new Mock<IFeatureManagementService>(MockBehavior.Strict);
            this.planMock = new Mock<IPlanRepository>(MockBehavior.Strict);
            this.versionMock = new Mock<IVersionRepository>(MockBehavior.Strict);
            this.actionRepoMock = new Mock<IActionRepository>(MockBehavior.Strict);
            this.builderRepoMock = new Mock<IBuilderRepository>(MockBehavior.Strict);
            this.fileStorageServiceMock = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Strict);
            this.mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            this.mockUserRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            this.mockCountryConfiguration = new Mock<CountryConfiguration>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<PublishController>>();
            this.mockPublishProjectService = new Mock<IPublishProjectService>();
            this.mockPublish3DcService = new Mock<IPublish3DcService>();


            controller = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger,
                mockPublishProjectService);
        }

        #region PublishOn
        /// <summary>
        /// Publish Plan by Master Version ID, passing a Plan ID. This will search the MasterVersion ID and send 
        /// this Version ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanTestAsync()
        {
            

            var publishPlanPostModel = new PublishPlanPostModel
            {
                PlanId = existingPlanId
            };
            var response = await this.controller.PublishPlan(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Publish Plan by Master Version ID with no ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanNullTestAsync()
        {
            var publishPlanPostModel = new PublishPlanPostModel();
            var response = await this.controller.PublishPlan(publishPlanPostModel) as BadRequestObjectResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Publish Plan by Master Version ID with and ID that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanNonExistingPlanIdTestAsync()
        {
            var publishPlanPostModel = new PublishPlanPostModel()
            {
                PlanId = nonExistingId
            };
            var response = await this.controller.PublishPlan(publishPlanPostModel) as NotFoundResult;
            Assert.NotNull(response);
        }

        /// <summary>
        /// Publish Plan by Master Version ID with no ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanInvalidCommentsTestAsync()
        {
            var publishPlanPostModel = new PublishPlanPostModel()
            {
                PlanId = 0,
            };
            var response = await this.controller.PublishPlan(publishPlanPostModel) as BadRequestObjectResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Publish Plan by Master Version ID with no ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanValidCommentsTestAsync()
        {
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            var publishPlanPostModel = new PublishPlanPostModel()
            {
                PlanId = existingPlanId,
            };
            var response = await this.controller.PublishPlan(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Publish Version by existing Version ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionTestAsync()
        {
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            var publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = ""
            };
            var response = await this.controller.PublishVersion(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Publish Version by existing Version ID and no html markup
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionValidCommentsTestAsync()
        {
            var publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "string contains no html markup"
            };
            var response = await this.controller.PublishVersion(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Publish Version by existing Version ID with suspected html markup
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionInvalidCommentsTestAsync()
        {
            var publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "string contains suspected <b> html markup"
            };
            var response = await this.controller.PublishVersion(publishPlanPostModel) as BadRequestObjectResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Publish Version with no ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionNullTestAsync()
        {
            var publishPlanPostModel = new PublishVersionModel();
            var response = await this.controller.PublishVersion(publishPlanPostModel) as BadRequestObjectResult;
            Assert.NotNull(response);
        }
        /// <summary>
        /// Publish Version with cycles not in the comments, the flag is off and IsCycles is false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_NoCyclesCommentFlagOffIsCyclesFalseAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

           PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger,
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "",
                IsCycles = false
                
            };

            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;
            
            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);

        }

        
        /// <summary>
        /// Publish Version with cycles in the comments, the flag is off and IsCycles is true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_CyclesCommentFlagOffIsCyclesfalseAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            PublishController pController  = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger,
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "cycles",
                IsCycles = false

            };
            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }

        /// <summary>
        /// Publish Version with cycles not in the comments, the flag is on and IsCycles is false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_NoCyclesCommentFlagOnIsCyclesfalseAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger,
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "",
                IsCycles = false

            };
            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }

        /// <summary>
        /// Publish Version with cycles not in the comments and the flag is off and IsCycles is true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_NoCyclesCommentFlagOffIsCyclesTrueAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger, 
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "",
                IsCycles = true

            };
            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }

        /// <summary>
        /// Publish Version with cycles in the comments, the flag on and IsCycles is false.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_CyclesCommentFlagOnIsCyclesfalseAsync_ReturnsTrue()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            this.actionRepoMock.Setup(x => x.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, "Cycles", It.IsAny<int>(),It.IsAny<string>())).Returns(Task.CompletedTask);

            this.mockLogger.MockLog(LogLevel.Information);

            PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger, 
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "cycles",
                IsCycles = false

            };

            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.True(publishPlanPostModel.IsCycles);
            mockLogger.VerifyLogger(Times.Exactly(1), LogLevel.Information);
        }

        /// <summary>
        /// Publish Version with cycles in the comments, the flag off and IsCycles is true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_NoCyclesCommentFlagOnIsCyclestrueAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            this.actionRepoMock.Setup(x => x.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, "", It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

           PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger,
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "",
                IsCycles = true

            };

            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }

        /// <summary>
        /// Publish Version with cycles in the comments, the flag is on and IsCycles is true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_CyclesCommentFlagOffIsCyclestrueAsync_ReturnFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            this.actionRepoMock.Setup(x => x.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, "", It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

           PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger, 
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "cycles",
                IsCycles = true

            };

            //Act
            var response = await pController.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }



        /// <summary>
        /// Publish Version with cycles in the comments, the flag is on and IsCycles is true.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersion_CyclesCommentFlagOnIsCyclestrueAsync_ReturnsTrue()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            this.actionRepoMock.Setup(x => x.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, "Cycles", It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            this.mockLogger.MockLog(LogLevel.Information);

            PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger, 
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = "cycles",
                IsCycles = true

            };

            //Act
            var response = await this.controller.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.True(publishPlanPostModel.IsCycles);
            mockLogger.VerifyLogger(Times.Exactly(1), LogLevel.Information); 
        }

        [Fact]
        public async Task PublishVersion_NullCommentFlagOnIsCyclestrueAsync_ReturnsFalse()
        {
            //Arrange
            ResetMocks();

            this.mockFeatureManagementService.Setup(x => x.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
            this.mockFeatureManagementService.Setup(x => x.GetFeatureFlagAsync("dvCyclesEnabled", It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            this.actionRepoMock.Setup(x => x.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, "", It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            PublishController pController = MakePublishController(this.mockPublishService,
                this.mockFeatureManagementService,
                this.planMock,
                this.versionMock,
                this.actionRepoMock,
                this.builderRepoMock,
                this.fileStorageServiceMock,
                this.mockUserService,
                this.mockUserRepo,
                this.mockLogger, 
                mockPublishProjectService);

            PublishVersionModel publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = null,
                IsCycles = true

            };

            //Act
            var response = await this.controller.PublishVersion(publishPlanPostModel) as OkResult;

            //Assert
            Assert.NotNull(response);
            Assert.False(publishPlanPostModel.IsCycles);
        }

        /// <summary>
        /// Publish Version with an ID that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionNonExistingVersionIdTestAsync()
        {
            var publishPlanPostModel = new PublishVersionModel()
            {
                VersionId = nonExistingId
            };
            var response = await this.controller.PublishVersion(publishPlanPostModel) as NotFoundResult;
            Assert.NotNull(response);
        }
        #endregion


        private void ResetMocks()
        {
            this.mockPublishService.Reset();
            this.mockFeatureManagementService.Reset();
            this.planMock.Reset();
            this.versionMock.Reset();
            this.actionRepoMock.Reset();
            this.builderRepoMock.Reset();
            this.fileStorageServiceMock.Reset();
            this.mockUserService.Reset();
            this.mockUserRepo.Reset();
            this.mockCountryConfiguration.Reset();
            this.mockLogger.Reset();
            mockPublishProjectService.Reset();
        }
 

        private PublishController MakePublishController(Mock<IPublishService> PublishServiceMock, Mock<IFeatureManagementService> FeatureManagementMock,
                    Mock<IPlanRepository> planMock, Mock<IVersionRepository> versionMock, Mock<IActionRepository> actionRepoMock,
                    Mock<IBuilderRepository> builderRepoMock, Mock<IFileStorageService<AzureStorageConfiguration>> fileStorageServiceMock, Mock<IUserService> userServiceMock,
                    Mock<IUserRepository> userRepoMock, Mock<ILogger<PublishController>> loggerMock,
                    Mock<IPublishProjectService> publishProjectServiceMock) //This method allows the tests to provide their own custom setups when making the PublishController.   
        {
            if (PublishServiceMock.Setups.Count is 0)
            {
                PublishServiceMock.Setup(m => m.PublishPlanFileAsync(It.IsAny<PublishVersionModel>(), It.IsAny<Stream>())).ReturnsAsync(new RepositoryResponse<string>());
                PublishServiceMock.Setup(m => m.PublishVersionAsync(It.IsAny<PublishVersionModel>())).ReturnsAsync(new RepositoryResponse<string>());
                PublishServiceMock.Setup(service => service.CreateLegacyZipPlanFromStream(It.IsAny<Stream>())).ReturnsAsync(new MemoryStream());
            }

            mockPublish3DcService.Setup(m => m.PublishVersionAsync(It.IsAny<PublishVersionModel>())).ReturnsAsync(new RepositoryResponse<string>());


            if (FeatureManagementMock.Setups.Count is 0)
            {
                FeatureManagementMock.Setup(a => a.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.On);
                FeatureManagementMock.Setup(x => x.GetFeatureFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);
            }

            if (planMock.Setups.Count is 0)
            {
                planMock.Setup(repo => repo.FindOneAsync<Plan>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int id, CancellationToken c) =>
                {
                    return (id == existingPlanId) ? new Plan() { MasterVersionId = existingVersionId, Id = existingPlanId } : null;
                });
                planMock.Setup(repo => repo.GetPlanWithHousingTypeHousingSpecs(It.IsAny<int>()))
                    .ReturnsAsync(new Plan());
            }

            if (versionMock.Setups.Count is 0)
            {
                versionMock.Setup(repo => repo.FindOneAsync<Domain.Entity.Version>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int id, CancellationToken c) =>
                {
                    return (id == existingVersionId) ? new Domain.Entity.Version { Id = existingVersionId, PlanId = existingPlanId } : null;
                });
                versionMock.Setup(repo => repo.GetRomPathById(It.IsAny<int>()))
                    .ReturnsAsync((int id) => (id == existingVersionId) ? new RepositoryResponse<string>() { Content = "plans\\my.rom" } : new RepositoryResponse<string>());
                versionMock.Setup(repo => repo.GetVersionWithPlanProjectAiep(It.IsAny<int>()))
                    .ReturnsAsync(new Version());
            }

            if (actionRepoMock.Setups.Count is 0)
            {
                actionRepoMock.Setup(repo => repo.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, string.Empty, It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(default(object)));
            }

            if (builderRepoMock.Setups.Count is 0)
            {
                builderRepoMock.Setup(b => b.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int x, CancellationToken c) =>
                {
                    return new Builder();
                });
            }

            if (fileStorageServiceMock.Setups.Count is 0)
            {
                fileStorageServiceMock.Setup(service => service.DownloadAsync(It.IsAny<string>())).ReturnsAsync(new MemoryStream());
            }

            if (userServiceMock.Setups.Count is 0)
            {
                mockUserService.Setup(m => m.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(1);
                mockUserService.Setup(m => m.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(1);
                mockUserService.Setup(x => x.GetUserIdentifier(It.IsAny<ClaimsIdentity>())).Returns("User");
            }

            if (userRepoMock.Setups.Count is 0)
            {
                mockUserRepo.Setup(u => u.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new RepositoryResponse<UserModel>() { Content = new UserModel() });
            }

            if (publishProjectServiceMock.Setups.Count is 0)
            {
                publishProjectServiceMock.Setup(p => p.SendRomItemsToCreatioAsync(It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(new RepositoryResponse<string>() { Content = "abc" });                   
            }

            loggerMock.MockLog(LogLevel.Debug);
            
            var countryConfigurationMock = new Mock<IOptions<CountryConfiguration>>(MockBehavior.Strict);
            CountryConfiguration app = new CountryConfiguration() { StrategyIdentifier = "IRL" };
            countryConfigurationMock.Setup(x => x.Value).Returns(app);


            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
                        }, "mock"));


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(PlanModelProfile));
                cfg.AddProfile(typeof(VersionModelProfile));

            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            var mockProjectRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockProjectRepo.Setup(m => m.GetProjectPlans(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>() { Content = new List<PlanModel>() });

   
            PublishController controller = new PublishController(
                PublishServiceMock.Object,
                planMock.Object,
                versionMock.Object,
                actionRepoMock.Object,
                loggerMock.Object,
                mockUserService.Object,
                FeatureManagementMock.Object,
                countryConfigurationMock.Object,
                mapper.Object, 
                mockPublishProjectService.Object,
                mockProjectRepo.Object,
                mockPublish3DcService.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } }
            };
            return controller;
        }
    }
    
}
