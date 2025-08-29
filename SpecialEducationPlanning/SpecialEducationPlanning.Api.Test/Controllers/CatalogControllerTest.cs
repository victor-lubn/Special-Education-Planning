using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class CatalogControllerTest : BaseTest
    {
        private readonly int existingCatalogId = 1;
        private readonly int nonExistingCatalogId = 99;

        private readonly Mock<ICatalogRepository> _mockCatalogRepository;
        private readonly Mock<ILogger<CatalogController>> _mockLogger;
        private readonly Mock<IFeatureManagementService> _mockFeatureManagementService;

        private readonly CatalogController catalogController;

        public CatalogControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(CatalogModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            _mockCatalogRepository = new Mock<ICatalogRepository>(MockBehavior.Strict);
            _mockFeatureManagementService = new Mock<IFeatureManagementService>(MockBehavior.Strict);
            _mockFeatureManagementService.Setup(a => a.GetFeatureFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            catalogController = new CatalogController(
                _mockCatalogRepository.Object,
                this.LoggerFactory.CreateLogger<CatalogController>(),
                mapper.Object,
                _mockFeatureManagementService.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() }
            };
        }

        #region Private Methods
        private CatalogModel CatalogInstance()
        {
            var model = new CatalogModel()
            {
                Id = existingCatalogId
            };
            return model;
        }

        private ICollection<CatalogModel> CatalogListInstance()
        {
            ICollection<CatalogModel> catalogs = new List<CatalogModel>
            {
                CatalogInstance()
            };
            return catalogs;
        }

        private Catalog CatalogInstanceEntity()
        {
            var entity = new Catalog()
            {
                Id = existingCatalogId
            };
            return entity;
        }

        private ICollection<Catalog> CatalogListInstanceEntitiy()
        {
            ICollection<Catalog> catalogs = new List<Catalog>
            {
                CatalogInstanceEntity()
            };
            return catalogs;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingCatalog_NotFound()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.FindOneAsync<Catalog>(nonExistingCatalogId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await catalogController.DeleteAsync(nonExistingCatalogId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingArea_Ok()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.FindOneAsync<Catalog>(existingCatalogId, CancellationToken.None))
                .ReturnsAsync(CatalogInstanceEntity());

            _mockCatalogRepository.Setup(cr => cr.Remove(existingCatalogId))
                .Verifiable();

            // Act
            var result = await catalogController.DeleteAsync(existingCatalogId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingCatalog_NotFound()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.GetCatalogById(nonExistingCatalogId))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>((CatalogModel)null));

            // Act
            var result = await catalogController.Get(nonExistingCatalogId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCatalogRepository.Verify(cr => cr.GetCatalogById(nonExistingCatalogId), Times.Once);
        }

        [Fact]
        public async void Get_ExistingCatalog_Ok()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.GetCatalogById(existingCatalogId))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(CatalogInstance()));

            // Act
            var result = await catalogController.Get(existingCatalogId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCatalogRepository.Verify(cr => cr.GetCatalogById(existingCatalogId), Times.Once);
        }
        #endregion

        #region Get Code from Catalog
        [Theory]
        [InlineData("Kitchens(19104)", "1458299216", "Fusion", "Fusion")]
        [InlineData("Kitchens(19104)", "1458299216", "", "Fusion")]
        [InlineData("Kitchens(19104)", "1458299216", null, "Fusion")]
        [InlineData("Balham(19103)", "1497616168", "3DC", "3DC")]
        public void GetCodeFromCatalogue_ValidCatalog_Ok(string name, string code, string queryEducationOrigin, string expectedEducationOrigin)
        {
            // Arrange
            CatalogModel model = CatalogInstance();
            model.Name = name;
            model.Code = code;
            model.EducationOrigin = expectedEducationOrigin;

            _mockCatalogRepository.Setup(repo => repo.GetCodeFromCatalogue(name, expectedEducationOrigin))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(model));

            // Act
            var response = catalogController.GetCodeFromCatalogue(name, queryEducationOrigin);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(code, ((CatalogModel)(response.Result as OkObjectResult).Value).Code);
            _mockCatalogRepository.Verify(repo => repo.GetCodeFromCatalogue(name, expectedEducationOrigin), Times.Once);
        }

        [Theory]
        [InlineData("4861467834", "Fusion")]
        [InlineData("3126457516", "3DC")]
        public void GetCodeFromCatalogue_InValidCatalog_BadRequest(string name, string EducationOrigin)
        {
            // Arrange
            _mockCatalogRepository.Setup(repo => repo.GetCodeFromCatalogue(name, EducationOrigin))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(ErrorCode.NoResults.GetDescription()));

            // Act
            var response = catalogController.GetCodeFromCatalogue(name, EducationOrigin);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }
        #endregion

        #region Get Catalog from Code
        [Theory]
        [InlineData("Kitchens(19104)", "1458299216", "Fusion", "Fusion")]
        [InlineData("Kitchens(19104)", "1458299216", "", "Fusion")]
        [InlineData("Kitchens(19104)", "1458299216", null, "Fusion")]
        [InlineData("Balham(19103)", "1497616168", "3DC", "3DC")]
        public void GetCatalogueFromCode_ValidCode_Ok(string name, string code, string queryEducationOrigin, string expectedEducationOrigin)
        {
            // Arrange
            CatalogModel model = CatalogInstance();
            model.Name = name;
            model.Code = code;

            _mockCatalogRepository.Setup(repo => repo.GetCatalogueFromCode(code, expectedEducationOrigin))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(model));

            // Act
            var response = catalogController.GetCatalogueFromCode(code, queryEducationOrigin);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(name, ((CatalogModel)(response.Result as OkObjectResult).Value).Name);
            _mockCatalogRepository.Verify(repo => repo.GetCatalogueFromCode(code, expectedEducationOrigin), Times.Once);
        }

        [Theory]
        [InlineData("4861467834", "Fusion")]
        [InlineData("3126457516", "3DC")]
        public void GetCatalogueFromCode_InValidCode_BadRequest(string name, string EducationOrigin)
        {
            // Arrange
            _mockCatalogRepository.Setup(repo => repo.GetCatalogueFromCode(name, EducationOrigin))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(ErrorCode.NoResults.GetDescription()));

            // Act
            var response = catalogController.GetCatalogueFromCode(name, EducationOrigin);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
            _mockCatalogRepository.Verify(repo => repo.GetCatalogueFromCode(name, EducationOrigin), Times.Once);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.GetCatalogs(EducationOriginType.Fusion.GetDescription()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<CatalogModel>>(CatalogListInstance()));

            // Act
            var result = await catalogController.GetAll(EducationOriginType.Fusion.GetDescription());

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(false, "Fusion")]
        [InlineData(true, "3DC")]
        public async void GetAll_EducationOriginIsNull_OkResulAndCallGetCatalogsWithCorrectEducationOrigin(bool flagProToolEnabled, string expectedEducationOrigin)
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.GetCatalogs(expectedEducationOrigin))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<CatalogModel>>(CatalogListInstance()));
            _mockFeatureManagementService.Setup(a => a.GetFeatureFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(flagProToolEnabled);

            // Act
            var result = await catalogController.GetAll(null);

            // Assert
            Assert.NotNull(result);
            _mockCatalogRepository.Verify(cr => cr.GetCatalogs(expectedEducationOrigin), Times.Once);
        }

        #endregion

        #endregion

        #region Post

        #region Post by Model
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            catalogController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await catalogController.PostAsync(CatalogInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ValidModel_Ok()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.CreateCatalog(It.IsAny<CatalogModel>()))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(CatalogInstance()));

            // Act
            var result = await catalogController.PostAsync(CatalogInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<CatalogModel>((result as OkObjectResult).Value);
        }
        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            catalogController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await catalogController.PutAsync(It.IsAny<int>(), It.IsAny<CatalogModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingCatalog_NotFound()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.CheckIfExistsAsync(nonExistingCatalogId))
                .ReturnsAsync(false);

            _mockCatalogRepository.Setup(cr => cr.ApplyChangesAsync(It.IsAny<Catalog>(), CancellationToken.None))
                .ReturnsAsync(CatalogInstanceEntity());

            _mockCatalogRepository.Setup(cr => cr.FindOneAsync<Catalog>(nonExistingCatalogId, CancellationToken.None))
                .ReturnsAsync(() => null);

            _mockCatalogRepository.Setup(cr => cr.GetCatalogById(existingCatalogId))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(CatalogInstance()));

            // Act  
            var result = await catalogController.PutAsync(nonExistingCatalogId, It.IsAny<CatalogModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCatalogRepository.Verify(cr => cr.GetCatalogById(existingCatalogId), Times.Never);
        }

        [Fact]
        public async void Put_ExistingCatalog_Ok()
        {
            // Arrange
            _mockCatalogRepository.Setup(cr => cr.CheckIfExistsAsync(existingCatalogId))
                .ReturnsAsync(true);

            _mockCatalogRepository.Setup(cr => cr.ApplyChangesAsync(It.IsAny<Catalog>(), CancellationToken.None))
            .ReturnsAsync(CatalogInstanceEntity());

            _mockCatalogRepository.Setup(cr => cr.FindOneAsync<Catalog>(existingCatalogId, CancellationToken.None))
                .ReturnsAsync(CatalogInstanceEntity());

            _mockCatalogRepository.Setup(cr => cr.GetCatalogById(existingCatalogId))
                .ReturnsAsync(new RepositoryResponse<CatalogModel>(CatalogInstance()));

            // Act
            var result = await catalogController.PutAsync(existingCatalogId, CatalogInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCatalogRepository.Verify(cr => cr.GetCatalogById(existingCatalogId), Times.Once);
        }
        #endregion

        #endregion

        #endregion
    }
}
