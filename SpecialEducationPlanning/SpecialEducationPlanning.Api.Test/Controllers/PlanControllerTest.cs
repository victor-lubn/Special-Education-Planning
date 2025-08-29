using System;
using System.Collections.Generic;
using System.Security.Claims;

using Koa.Persistence.Abstractions.QueryResult;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Api.Model.AutomaticArchiveModel;
using SpecialEducationPlanning
.Business.Mapper;


using Moq;
using Xunit;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Api.Service.Search;
using Xunit.Abstractions;
using System.Threading;
using Koa.Domain.Search.Page;
using Koa.Serialization.Json;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Business.Repository.HouseTypeRepository;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class PlanControllerTest : BaseTest
    {
        private const int existingPlanId = 1;
        private const int nonExistingPlanId = 99;

        private readonly int existingUserId = 1;
        private readonly int existingEducationerId = 1;

        private readonly int existingAiepId = 1;
        private readonly int nonExistingAiepId = 99;

        private readonly int existingBuilderId = 1;
        private readonly int nonExistingBuilderId = 99;

        private readonly int existingCatalogueId = 1;
        private readonly int nonExistingCatalogueId = 99;

        private readonly string validPostCode = "PE26 1DQ";

        private readonly string userFullName = "Stephen Dunn";

        private readonly Mock<IPlanRepository> _mockPlanRepository;
        private readonly Mock<IEducationerRepository> _mockEducationerRepository;
        private readonly Mock<IBuilderRepository> _mockBuilderRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IAiepRepository> _mockAiepRepository;
        private readonly Mock<IEndUserRepository> _mockEndUserRepository;
        private readonly Mock<IActionRepository> _mockActionRepository;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IVersionRepository> _mockVersionRepository;
        private readonly Mock<ICatalogRepository> _mockCatalogRepository;
        private readonly Mock<IBuilderEducationerAiepRepository> _mockBuilderEducationerAiepRepository;
        private readonly Mock<IRomItemRepository> _mockRomItemRepository;
        private readonly Mock<IFileStorageService<AzureStorageConfiguration>> _mockFileStorageService;
        private readonly Mock<IOptions<AutomaticArchiveConfiguration>> _mockOptions;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPostCodeServiceFactory> _mockPostCodeServiceFactory;
        private readonly Mock<IOptions<AzureSearchConfiguration>> _mockAzureSearchConfiguration;
        private readonly Mock<IAzureSearchService> _mockAzureSearchService;
        private readonly Mock<IFeatureManagementService> _mockFeatureManagementService;
        //
        private readonly Mock<IServiceProvider> serviceProviderMock;
        private readonly Mock<HttpContext> httpContextMock;

        private readonly PlanController planController;

        public PlanControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockPlanRepository = new Mock<IPlanRepository>(MockBehavior.Strict);
            _mockEducationerRepository = new Mock<IEducationerRepository>(MockBehavior.Strict);
            _mockBuilderRepository = new Mock<IBuilderRepository>(MockBehavior.Strict);
            _mockProjectRepository = new Mock<IProjectRepository>(MockBehavior.Strict);
            _mockAiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            _mockEndUserRepository = new Mock<IEndUserRepository>(MockBehavior.Strict);
            _mockActionRepository = new Mock<IActionRepository>(MockBehavior.Loose);
            _mockCommentRepository = new Mock<ICommentRepository>(MockBehavior.Strict);
            _mockVersionRepository = new Mock<IVersionRepository>(MockBehavior.Strict);
            _mockCatalogRepository = new Mock<ICatalogRepository>(MockBehavior.Default);
            _mockBuilderEducationerAiepRepository = new Mock<IBuilderEducationerAiepRepository>(MockBehavior.Strict);
            _mockRomItemRepository = new Mock<IRomItemRepository>(MockBehavior.Strict);
            _mockFileStorageService = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Strict);
            _mockOptions = new Mock<IOptions<AutomaticArchiveConfiguration>>(MockBehavior.Default);
            _mockUserService = new Mock<IUserService>(MockBehavior.Default);
            _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            _mockPostCodeServiceFactory = new Mock<IPostCodeServiceFactory>(MockBehavior.Strict);
            _mockAzureSearchConfiguration = new Mock<IOptions<AzureSearchConfiguration>>();
            _mockAzureSearchService = new Mock<IAzureSearchService>(MockBehavior.Strict);
            _mockFeatureManagementService = new Mock<IFeatureManagementService>(MockBehavior.Strict);

            _mockFeatureManagementService.Setup(a => a.GetFeatureFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            this.serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            this.httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>(MockBehavior.Strict);
            httpContextMock.SetupGet(context => context.RequestServices).Returns(this.serviceProviderMock.Object);



            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(PlanModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());


            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            httpContextMock.SetupGet(context => context.User).Returns(user);

            planController = new PlanController(
                _mockPlanRepository.Object,
                _mockEducationerRepository.Object,
                _mockBuilderRepository.Object,
                _mockProjectRepository.Object,
                _mockAiepRepository.Object,
                _mockEndUserRepository.Object,
                _mockActionRepository.Object,
                _mockCommentRepository.Object,
                _mockVersionRepository.Object,
                _mockCatalogRepository.Object,
                _mockBuilderEducationerAiepRepository.Object,
                _mockRomItemRepository.Object,
                _mockFileStorageService.Object,
                _mockOptions.Object,
                _mockUserService.Object,
                _mockUserRepository.Object,
                _mockPostCodeServiceFactory.Object,
            this.LoggerFactory.CreateLogger<PlanController>(),
                _mockAzureSearchConfiguration.Object,
                _mockAzureSearchService.Object,
                mapper.Object,
                _mockFeatureManagementService.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = this.httpContextMock.Object }
            };
        }

        #region Private Methods
        private PlanModel PlanExistingInstance()
        {
            var model = new PlanModel
            {
                Id = existingPlanId,
                CatalogId = 1
            };
            return model;
        }

        private Plan PlanExistingInstanceEntity()
        {
            var model = new Plan
            {
                Id = existingPlanId,
                CatalogId = 1
            };
            return model;
        }
        private ICollection<PlanModel> PlanListInstance()
        {
            ICollection<PlanModel> plans = new List<PlanModel>
            {
                PlanExistingInstance()
            };
            return plans;
        }

        private ICollection<VersionModel> VersionListInstance()
        {
            ICollection<VersionModel> versions = new List<VersionModel>
            {
                new VersionModel()
            };
            return versions;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingPlan_NotFound()
        {
            // Arrange
            _mockPlanRepository.Setup(ar => ar.FindOneAsync<Plan>(nonExistingPlanId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act  
            var result = await planController.Delete(nonExistingPlanId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingPlan_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(ar => ar.FindOneAsync<Plan>(existingPlanId, CancellationToken.None))
                .ReturnsAsync(PlanExistingInstanceEntity());

            _mockPlanRepository.Setup(ar => ar.Remove(existingPlanId))
                .Verifiable();

            // Act 
            var result = await planController.Delete(existingPlanId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #region Delete Empty Plans
        [Fact]
        public async void DeleteEmptyPlans_NonExistingPlan_NotFound()
        {
            // Arrange
            _mockPlanRepository.Setup(ar => ar.DeleteEmptyPlans())
                .ReturnsAsync(false);

            // Act  
            var result = await planController.DeleteEmptyPlans();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void DeleteEmptyPlans_ExistingPlan_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(ar => ar.DeleteEmptyPlans())
                .ReturnsAsync(true);

            // Act 
            var result = await planController.DeleteEmptyPlans();

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Generate Plan Code
        [Fact]
        public async void GeneratePlanId_Ok()
        {
            // Arrange
            var planCode = "10010001000";

            _mockPlanRepository.Setup(pr => pr.GeneratePlanIdAsync(null))
                .ReturnsAsync(new RepositoryResponse<string>() { Content = planCode });

            // Act
            var result = await planController.GeneratePlanId();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(planCode, (result as OkObjectResult).Value);
        }
        #endregion

        #region Get by ID
        [Fact]
        public async void Get_ExistingPlanFullACL_Ok()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingUserId);

            _mockUserRepository.Setup(ur => ur.FindOneAsync<User>(existingUserId, CancellationToken.None))
                .ReturnsAsync(new User() { FullAclAccess = true });

            _mockPlanRepository.Setup(pr => pr.GetPlanAsync(existingPlanId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>(PlanExistingInstance()));

            // Act
            var result = await planController.Get(existingPlanId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_ExistingPlanWithoutArchived_Ok()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingUserId);

            _mockUserRepository.Setup(ur => ur.FindOneAsync<User>(existingUserId, CancellationToken.None))
                .ReturnsAsync(new User() { FullAclAccess = false });

            _mockPlanRepository.Setup(pr => pr.GetPlanAsync(existingPlanId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>(PlanExistingInstance()));

            // Act
            var result = await planController.Get(existingPlanId) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_NonExistingPlan_NotFound()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingUserId);

            _mockUserRepository.Setup(ur => ur.FindOneAsync<User>(existingUserId, CancellationToken.None))
                .ReturnsAsync(new User() { FullAclAccess = false });

            _mockPlanRepository.Setup(pr => pr.GetPlanAsync(existingPlanId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>((PlanModel)null));

            // Act
            var result = await planController.Get(existingPlanId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        // To do
        #region Get Actions

        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 
            _mockPlanRepository.Setup(ar => ar.GetAllPlansWithoutArchivedPlansAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>() { Content = PlanListInstance() });

            // Act
            var result = await planController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PlanModel>>((result as OkObjectResult).Value);
        }
        #endregion

        #region Get All With Archived
        [Fact]
        public async void GetAllWithArchived_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.GetAllPlansWithArchivedPlansAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>() { Content = PlanListInstance() });

            // Act
            var result = await planController.GetAllWithArchived();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PlanModel>>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void GetAllArchivedPlans_ModelError_BadRequest()
        {
            // Arrange
            planController.ModelState.AddModelError("id", "id is null");

            // Act 
            var result = await planController.GetAllArchivedPlans(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetAllArchivedPlans_ValidModel_Ok()
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

            _mockPlanRepository.Setup(pr => pr.GetAllArchivedPlansAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<PlanModel>>()
                {
                    Content = new PagedQueryResult<PlanModel>(
                        PlanListInstance(),
                        searchModel.Take,
                        searchModel.Skip,
                        10)
                });

            // Act
            var result = await planController.GetAllArchivedPlans(searchModel);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetAllArchivedPlans_ErrorFound_Ok()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 100,
                Skip = 0,
            };

            _mockPlanRepository.Setup(pr => pr.GetAllArchivedPlansAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<PlanModel>>() { ErrorList = { ErrorCode.NoResults.GetDescription() } });

            // Act
            var result = await planController.GetAllArchivedPlans(searchModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion

        // To do
        #region GetPlanActions

        #endregion

        // To do
        #region GetPlansFiltered

        #endregion

        // To do
        #region Get Plans Sorted

        #endregion

        #region Get Plan Versions
        [Fact]
        public async void GetPlanVersion_NonExistingPlan_BadRequest()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.GetPlanVersions(existingPlanId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetPlanVersions_ExistingPlan_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(true);

            _mockVersionRepository.Setup(vr => vr.GetVersionsByPlanId(existingPlanId))
                .ReturnsAsync(VersionListInstance());

            // Act
            var result = await planController.GetPlanVersions(existingPlanId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        // To do
        #region Get Comments

        #endregion

        #endregion

        #region Post

        // To do
        #region Assign Builder To Plan

        #endregion

        // To do
        #region Unassign Builder From Plan

        #endregion

        // To do
        #region Automatic Archive

        #endregion

        #region Change Plan State
        [Fact]
        public async void ChangePlanState_NonExistingPlan_NotFound()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.FindOneAsync<Plan>(nonExistingPlanId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act  
            var result = await planController.ChangePlanState(nonExistingPlanId, PlanState.Active);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void ChangePlanState_NoAction_Ok()
        {
            // Arrange
            var planState = PlanState.Active;

            _mockPlanRepository.Setup(pr => pr.FindOneAsync<Plan>(existingPlanId, CancellationToken.None))
                .ReturnsAsync(PlanExistingInstanceEntity);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockPlanRepository.Setup(pr => pr.ChangePlanStateAsync(It.IsAny<PlanModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = null });

            // Act  
            var result = await planController.ChangePlanState(existingPlanId, planState);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ChangePlanState_Ok()
        {
            // Arrange
            var planState = PlanState.Active;

            _mockPlanRepository.Setup(pr => pr.FindOneAsync<Plan>(existingPlanId, CancellationToken.None))
                .ReturnsAsync(PlanExistingInstanceEntity());

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockPlanRepository.Setup(pr => pr.ChangePlanStateAsync(It.IsAny<PlanModel>(), planState))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = PlanExistingInstance() });

            // Act  
            var result = await planController.ChangePlanState(existingPlanId, planState);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        // To do
        #region Copy Plan

        #endregion

        #region Transfer Single Plan Between Aieps
        [Fact]
        public async void TransferSinglePlanBetweenAieps_NoPlan_BadRequest()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(nonExistingPlanId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.TransferSinglePlanBetweenAieps(nonExistingPlanId, existingAiepId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void TransferSinglePlanBetweenAieps_NoAiep_BadRequest()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(true);

            _mockAiepRepository.Setup(pr => pr.CheckIfExistsAsync(nonExistingAiepId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.TransferSinglePlanBetweenAieps(existingPlanId, nonExistingAiepId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void TransferSinglePlanBetweenAieps_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(true);

            _mockAiepRepository.Setup(pr => pr.CheckIfExistsAsync(existingAiepId))
                .ReturnsAsync(true);

            _mockPlanRepository.Setup(pr => pr.TransferSinglePlanBetweenAieps(existingPlanId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = PlanExistingInstance() });

            // Act
            var result = await planController.TransferSinglePlanBetweenAieps(existingPlanId, existingAiepId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PlanModel>((result as OkObjectResult).Value);
        }
        #endregion

        #region Transfer Multiple Plan Between Aieps
        [Fact]
        public async void TransferMultiplePlanBetweenAieps_NoBullder_BadRequest()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.CheckIfExistsAsync(nonExistingBuilderId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.TransferMultiplePlanBetweenAieps(nonExistingBuilderId, existingAiepId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void TransferMultiplePlanBetweenAieps_NoAiep_BadRequest()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.CheckIfExistsAsync(existingBuilderId))
                .ReturnsAsync(true);

            _mockAiepRepository.Setup(br => br.CheckIfExistsAsync(nonExistingAiepId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.TransferMultiplePlanBetweenAieps(existingBuilderId, nonExistingAiepId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void TransferMultiplePlanBetweenAieps_Ok()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.CheckIfExistsAsync(existingBuilderId))
                .ReturnsAsync(true);

            _mockAiepRepository.Setup(br => br.CheckIfExistsAsync(existingAiepId))
                .ReturnsAsync(true);

            _mockPlanRepository.Setup(pr => pr.TransferMultiplePlanToUnassignedBuilder(existingBuilderId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>() { Content = PlanListInstance() });

            // Act
            var result = await planController.TransferMultiplePlanBetweenAieps(existingBuilderId, existingAiepId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PlanModel>>((result as OkObjectResult).Value);
        }
        #endregion

        // To do
        #region Post Comment

        #endregion

        #region Post Plan
        [Fact]
        public async void PostPlan_ModelError_BadRequest()
        {
            // Arrange
            planController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await planController.PostSinglePlan(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PostPlan_NoCatalogue_BadRequest()
        {
            // Arrange
            _mockCatalogRepository.Setup(pr => pr.CheckIfExistsAsync(nonExistingCatalogueId))
                .ReturnsAsync(false);

            // Act
            var result = await planController.PostSinglePlan(PlanExistingInstance());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostPlan_ValidCatalogue_NoAiepId_BadRequest()
        {
            // Arrange
            _mockCatalogRepository.Setup(pr => pr.CheckIfExistsAsync(existingCatalogueId))
                .ReturnsAsync(true);

            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(null);

            // Act
            var result = await planController.PostSinglePlan(PlanExistingInstance());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostPlan_ValidCatalogue_ExistingAiepId_EndUserEntityNotFound()
        {
            // Arrange
            _mockCatalogRepository.Setup(pr => pr.CheckIfExistsAsync(existingCatalogueId))
                .ReturnsAsync(true);

            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingAiepId);

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingUserId);

            _mockPlanRepository.Setup(pr => pr.CreatePlan(It.IsAny<PlanModel>(), existingAiepId, existingUserId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

            // Act
            var result = await planController.PostSinglePlan(PlanExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PostPlan_ValidCatalogue_ExistingAiepId_OkResult()
        {
            // Arrange
            _mockCatalogRepository.Setup(pr => pr.CheckIfExistsAsync(existingCatalogueId))
                .ReturnsAsync(true);

            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingAiepId);

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingUserId);

            _mockPlanRepository.Setup(pr => pr.CreatePlan(It.IsAny<PlanModel>(), existingAiepId, existingUserId))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = PlanExistingInstance() });

            // Act
            var result = await planController.PostSinglePlan(PlanExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            var model = PlanExistingInstance();
            model.EndUser = new EndUserModel() { Postcode = validPostCode };

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            planController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await planController.Put(PlanExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingPlan_NotFound()
        {
            // Arrange
            var model = PlanExistingInstance();
            model.Id = nonExistingPlanId;

            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(nonExistingPlanId))
                .ReturnsAsync(false);

            // Act  
            var result = await planController.Put(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockPlanRepository.Verify(pr => pr.CheckIfExistsAsync(nonExistingPlanId), Times.Once);
        }

        [Fact(Skip = "Can't make User null")]
        public async void Put_NullUser_BadRequest()
        {
            // Arrange 
            _mockPlanRepository.Setup(pr => pr.FindOneAsync<Plan>(nonExistingPlanId, CancellationToken.None))
                .ReturnsAsync(PlanExistingInstanceEntity());

            //planController.HttpContext.User.Identity = null;

            // Act  
            var result = await planController.Put(PlanExistingInstance());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_NoAction_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(true);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockPlanRepository.Setup(pr => pr.CreateOrUpdateAsync(It.IsAny<PlanModel>()))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = null });

            // Act  
            var result = await planController.Put(PlanExistingInstance());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Put_Ok()
        {
            // Arrange
            _mockPlanRepository.Setup(pr => pr.CheckIfExistsAsync(existingPlanId))
                .ReturnsAsync(true);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userFullName);

            _mockPlanRepository.Setup(pr => pr.CreateOrUpdateAsync(It.IsAny<PlanModel>()))
                .ReturnsAsync(new RepositoryResponse<PlanModel>() { Content = PlanExistingInstance() });

            // Act  
            var result = await planController.Put(PlanExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion

        #endregion

        #endregion

    }
}

