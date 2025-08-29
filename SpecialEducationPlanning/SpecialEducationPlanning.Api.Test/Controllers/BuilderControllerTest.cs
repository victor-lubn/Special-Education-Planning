using System;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.Sap;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

using Moq;

using SpecialEducationPlanning
.Api.Service.Search;

using Xunit;
using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Api.Configuration.AzureSearch;
using SpecialEducationPlanning
.Business.IService;
using Xunit.Abstractions;
using System.Threading;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class BuilderControllerTest : BaseTest
    {
        private readonly string validAccountNumber = "1000065857";

        private readonly int existingBuilderId = 1;
        private readonly int nonExistingBuilderId = 99;

        private readonly int existingUserId = 1;
        private readonly int existingAiepId = 1;

        private readonly string validPostCode = "PE26 1DQ";

        private readonly Mock<IBuilderEducationerAiepRepository> _mockBuilderEducationerAiepRepository;
        private readonly Mock<IBuilderRepository> _mockBuilderRepository;
        private readonly Mock<IAiepRepository> _mockAiepRepository;
        private readonly Mock<IPlanRepository> _mockPlanRepository;
        private readonly Mock<ISapService> _mockSapService;
        private readonly Mock<IPostCodeServiceFactory> _mockPostCodeServiceFactory;
        private readonly Mock<IOptions<SapConfiguration>> _mockOptions;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IAzureSearchService> _mockAzureSearchService;
        private readonly Mock<IOptions<AzureSearchConfiguration>> _mockAzureSearchConfiguration;

        private readonly BuilderController builderController;

        public BuilderControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockBuilderEducationerAiepRepository = new Mock<IBuilderEducationerAiepRepository>(MockBehavior.Strict);
            _mockBuilderRepository = new Mock<IBuilderRepository>(MockBehavior.Strict);
            _mockAiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            _mockPlanRepository = new Mock<IPlanRepository>(MockBehavior.Strict);
            _mockSapService = new Mock<ISapService>(MockBehavior.Strict);
            _mockPostCodeServiceFactory = new Mock<IPostCodeServiceFactory>(MockBehavior.Strict);
            _mockOptions = new Mock<IOptions<SapConfiguration>>(MockBehavior.Default);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            _mockAzureSearchService = new Mock<IAzureSearchService>(MockBehavior.Strict);
            _mockAzureSearchConfiguration = new Mock<IOptions<AzureSearchConfiguration>>();


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(BuilderModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            builderController = new BuilderController(
                _mockBuilderEducationerAiepRepository.Object,
                _mockBuilderRepository.Object,
                _mockAiepRepository.Object,
                _mockPlanRepository.Object,
                _mockSapService.Object,
                _mockPostCodeServiceFactory.Object,
                _mockOptions.Object,
                this.LoggerFactory.CreateLogger<BuilderController>(),
                _mockUserService.Object,
                _mockAzureSearchService.Object,
                _mockAzureSearchConfiguration.Object,
                mapper.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() } }
            };
        }

        #region Private Methods
        private BuilderModel BuilderExistingInstance()
        {
            var model = new BuilderModel
            {
                Id = existingBuilderId
            };
            return model;
        }

        private ICollection<BuilderModel> BuilderListInstance()
        {
            ICollection<BuilderModel> areas = new List<BuilderModel>
            {
                BuilderExistingInstance()
            };
            return areas;
        }

        private BuilderEducationerAiepModel BuilderEducationerAiepExistingInstance()
        {
            var model = new BuilderEducationerAiepModel
            {
                BuilderId = existingBuilderId,
                AiepId = existingAiepId
            };
            return model;
        }

        private Builder BuilderExistingInstanceEntity()
        {
            var model = new Builder
            {
                Id = existingBuilderId
            };
            return model;
        }

        private ICollection<Builder> BuilderListInstanceEntity()
        {
            ICollection<Builder> areas = new List<Builder>
            {
                BuilderExistingInstanceEntity()
            };
            return areas;
        }

        private BuilderEducationerAiep BuilderEducationerAiepExistingInstanceEntity()
        {
            var model = new BuilderEducationerAiep
            {
                BuilderId = existingBuilderId,
                AiepId = existingAiepId
            };
            return model;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingBuilder_NotFound()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.CheckIfExistsAsync(nonExistingBuilderId))
                .ReturnsAsync(false);

            // Act  
            var result = await builderController.Delete(nonExistingBuilderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingBuilder_Ok()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.CheckIfExistsAsync(existingBuilderId))
                .ReturnsAsync(true);

            _mockBuilderRepository.Setup(br => br.Remove(existingBuilderId))
                .Verifiable();

            // Act 
            var result = await builderController.Delete(existingBuilderId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #region Delete Account Number From Builder
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void DeleteAccountNumberFromBuilder_NullOrEmptyAccountNumber_BadRequest(string accountNumber)
        {
            // Act
            var result = await builderController.DeleteAccountNumberFromBuilder(accountNumber, existingBuilderId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void DeleteAccountNumberFromBuilder_NoBuilder_BadRequest()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.DeleteAccountNumberAsync(existingBuilderId, validAccountNumber))
                .ReturnsAsync(new RepositoryResponseGeneric());

            // Act
            var result = await builderController.DeleteAccountNumberFromBuilder(validAccountNumber, existingBuilderId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingBuilder_NotFound()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.FindOneAsync<Builder>(nonExistingBuilderId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await builderController.Get(nonExistingBuilderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingBuilder_Ok()
        {
            // Arrange
            _mockBuilderRepository.Setup(br => br.FindOneAsync<Builder>(existingBuilderId, CancellationToken.None))
                .ReturnsAsync(BuilderExistingInstanceEntity());

            // Act
            var result = await builderController.Get(existingBuilderId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 
            _mockBuilderRepository.Setup(ar => ar.GetAllAsync<Builder>(CancellationToken.None))
                .ReturnsAsync(BuilderListInstanceEntity());

            // Act
            var result = await builderController.GetAll();

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Get Builder from SAP
        [Fact]
        public async void GetBuilderFromSap_Ok()
        {
            // Arrange
            _mockSapService.Setup(ss => ss.GetSapBuilder(It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<List<BuilderModel>>() { Content = new List<BuilderModel>() { BuilderExistingInstance() } });

            // Act
            var result = await builderController.GetBuilderFromSap(It.IsAny<List<string>>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<BuilderModel>>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void GetBuilderFromSap_BuilderMandatoryFields_Ok()
        {
            // Arrange
            var surname = "Dunn";
            var address = "33 Hollow Lane, Ramsey, HUNTINGDON, Cambridgeshire";

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).RepresentPostcode(validPostCode))
                .Returns(validPostCode);

            _mockSapService.Setup(ss => ss.GetSapBuilderAsync(surname, validPostCode, address))
                .ReturnsAsync(new RepositoryResponse<List<BuilderModel>>() { Content = new List<BuilderModel>() { BuilderExistingInstance() } });

            // Act
            var result = await builderController.GetBuilderFromSap(surname, validPostCode, address);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        // To do
        #region Get BuilderModel If Exists

        #endregion

        // To do
        #region Get Builders Filtered

        #endregion

        // To do
        #region Get Possible Matching Builders
        [Fact]
        public async void GetPosibleMatchingBuilders_ModelError_BadRequest()
        {
            // Arrange
            builderController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await this.builderController.GetPossibleMatchingBuilders(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetPosibleMatchingBuilders_ErrorMatch_Ok()
        {
            // Assert
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(br => br.GetPosibleTdpMatchingBuilders(model))
                .ReturnsAsync(new RepositoryResponse<ValidationBuilderModel>() { Content = null });

            // Act
            var result = await this.builderController.GetPossibleMatchingBuilders(model);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void GetPosibleMatchingBuilders_ExactMatch_Ok()
        {
            // Assert
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(br => br.GetPosibleTdpMatchingBuilders(model))
                .ReturnsAsync(new RepositoryResponse<ValidationBuilderModel>() { Content = new ValidationBuilderModel() { Type = BuilderMatchType.Exact } });

            // Act
            var result = await this.builderController.GetPossibleMatchingBuilders(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(BuilderMatchType.Exact, (((result as OkObjectResult).Value) as ValidationBuilderModel).Type);
        }
        #endregion

        // To do
        #region Get Possible Matching Builder By AccountNumber

        #endregion

        // To do
        #region Move Builder

        #endregion

        #endregion

        #region Post

        #region Post by Model
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            builderController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await builderController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ExistingBuilder_BadRequest()
        {
            // Arrange 
            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = BuilderExistingInstance() });

            // Act
            var result = await builderController.Post(BuilderExistingInstance());

            // Arrange
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_NonValidPostCode_BadRequest()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(existingUserId);
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(existingAiepId);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                .Returns(new RepositoryResponse<string>() { Content = null });

            // Act
            var result = await builderController.Post(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public async void Post_ErrorCreate_BadRequest()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(existingUserId);
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(existingAiepId);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                .Returns(new RepositoryResponse<string>() { Content = validPostCode });

            _mockBuilderRepository.Setup(br => br.CreateAsync(It.IsAny<BuilderModel>(), existingUserId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

            // Act
            var result = await builderController.Post(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

        }

        [Fact]
        public async void Post_Ok()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(existingUserId);
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(existingAiepId);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                .Returns(new RepositoryResponse<string>() { Content = validPostCode });

            _mockBuilderRepository.Setup(br => br.CreateAsync(It.IsAny<BuilderModel>(), existingUserId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = BuilderExistingInstance() });

            // Act
            var result = await builderController.Post(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);

        }
        #endregion

        #endregion

        #region Put

        #region Assign Builder to current User Aiep
        [Fact]
        public async void AssignBuilderToCurrentUserAiep_NoExistingRelation_BadRequest()
        {
            // Arrange 
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(existingAiepId);

            _mockBuilderEducationerAiepRepository.Setup(bddr => bddr.GetBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<BuilderEducationerAiepModel>() { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });
            _mockBuilderEducationerAiepRepository.Setup(bddr => bddr.CreateBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<BuilderEducationerAiepModel>() { Content = BuilderEducationerAiepExistingInstance() });

            // Act
            var result = await this.builderController.AssignBuilderToCurrentUserAiep(existingBuilderId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void AssignBuilderToCurrentUserAiep_Ok()
        {
            // Arrange 
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(existingAiepId);

            _mockBuilderEducationerAiepRepository.Setup(bddr => bddr.GetBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId))
                .ReturnsAsync(new RepositoryResponse<BuilderEducationerAiepModel>() { Content = BuilderEducationerAiepExistingInstance() });

            // Act
            var result = await this.builderController.AssignBuilderToCurrentUserAiep(existingBuilderId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            builderController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await builderController.Put(It.IsAny<int>(), BuilderExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public async void Put_NoBuilder_NotFound(int builderId)
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Id = builderId;

            // Act
            var result = await builderController.Put(model.Id, model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingBuilder_BadRequest()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            var notSameBuilder = 2;

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>(BuilderExistingInstance()));

            // Act  
            var result = await builderController.Put(notSameBuilder, model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonValidPostCode_BadRequest()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                 .Returns(new RepositoryResponse<string>() { Content = null });

            // Act
            var result = await builderController.Put(model.Id, model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_PlanTradingNameError_Ok()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;
            model.TradingName = "Stephen Dunn";

            var entity = new Builder()
            {
                TradingName = model.TradingName,
                Postcode = model.Postcode,
                Id = model.Id
            };

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });


            _mockBuilderRepository.Setup(rr => rr.FindOneAsync<Builder>(existingBuilderId, CancellationToken.None))
    .ReturnsAsync(entity);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                 .Returns(new RepositoryResponse<string>() { Content = validPostCode });

            _mockBuilderRepository.Setup(br => br.ApplyChangesAsync(It.IsAny<Builder>(), CancellationToken.None))
                .ReturnsAsync(BuilderExistingInstanceEntity());

            _mockPlanRepository.Setup(pr => pr.UpdateBuilderPlansTradingName(model.Id, model.TradingName))
                .ReturnsAsync(false);

            // Act
            var result = await builderController.Put(model.Id, model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BuilderModel>((result as OkObjectResult).Value);
            Assert.Equal(model.TradingName, (((result as OkObjectResult).Value) as BuilderModel).TradingName);
        }

        [Fact]
        public async void Put_Ok()
        {
            // Arrange
            var model = BuilderExistingInstance();
            model.Postcode = validPostCode;
            model.TradingName = "Stephen Dunn";

            var entity = BuilderExistingInstanceEntity();
            entity.Postcode = validPostCode;
            entity.TradingName = "Stephen Dunn";

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).NormalisePostcode(validPostCode))
                .Returns(validPostCode);

            _mockBuilderRepository.Setup(rr => rr.FindOneAsync<Builder>(existingBuilderId, CancellationToken.None))
    .ReturnsAsync(BuilderExistingInstanceEntity());

            _mockBuilderRepository.Setup(br => br.GetExistingBuilderOrEmptyAsync(It.IsAny<BuilderModel>()))
                .ReturnsAsync(new RepositoryResponse<BuilderModel>() { Content = null });

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(It.IsAny<string>()))
                 .Returns(new RepositoryResponse<string>() { Content = validPostCode });

            _mockBuilderRepository.Setup(br => br.ApplyChangesAsync(It.IsAny<Builder>(), CancellationToken.None))
                .ReturnsAsync(entity);

            _mockPlanRepository.Setup(pr => pr.UpdateBuilderPlansTradingName(model.Id, model.TradingName))
                .ReturnsAsync(true);

            // Act
            var result = await builderController.Put(model.Id, model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<BuilderModel>((result as OkObjectResult).Value);
            Assert.Equal(model.TradingName, (((result as OkObjectResult).Value) as BuilderModel).TradingName);
        }
        #endregion

        // To do
        #region Modify Credit Builder Notes

        #endregion

        // To do
        #region Builder Right To Be Forgotten Two Year After

        #endregion

        // To do
        #region SAP Health Check

        #endregion

        // To do
        #region Update Builder from SAP by Account Number

        #endregion

        // To do
        #region Update SAP Builders

        #endregion

        // To do
        #region Unassign Account Number

        #endregion

        #endregion

        #endregion
    }
}

