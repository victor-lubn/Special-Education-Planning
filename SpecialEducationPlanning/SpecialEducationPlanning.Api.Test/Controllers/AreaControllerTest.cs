using System;
using System.Collections.Generic;

using Koa.Persistence.Abstractions.QueryResult;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.DtoModel;

using Moq;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using Koa.Domain.Search.Page;
using Koa.Serialization.Json;
using Microsoft.AspNetCore.Http;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class AreaControllerTest : BaseTest
    {
        private readonly int existingAreaId = 1;
        private readonly int nonExistingAreaId = 99;

        private readonly Mock<IAreaRepository> _mockAreaRepository;
        private readonly Mock<IServiceProvider> serviceProviderMock;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> httpContextMock;

        private readonly AreaController areaController;

        public AreaControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

            _mockAreaRepository = new Mock<IAreaRepository>(MockBehavior.Strict);

            this.serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            this.httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>(MockBehavior.Strict);
            this.httpContextMock.SetupGet(context => context.RequestServices).Returns(this.serviceProviderMock.Object);


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AreaModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            areaController = new AreaController(
                _mockAreaRepository.Object,
                this.LoggerFactory.CreateLogger<AreaController>(),
                mapper.Object
            )
            {
                ControllerContext = new ControllerContext() { HttpContext = this.httpContextMock.Object }
            };
        }

        #region Private Methods
        private Area AreaInstance()
        {
            var model = new Area
            {
                Id = existingAreaId
            };
            return model;
        }

        private ICollection<Area> AreaListInstance()
        {
            ICollection<Area> areas = new List<Area>
            {
                AreaInstance()
            };
            return areas;
        }

        private AreaModel AreaInstanceModel()
        {
            var model = new AreaModel
            {
                Id = existingAreaId
            };


            return model;
        }

        private ICollection<AreaModel> AreaListInstanceModel()
        {
            ICollection<AreaModel> areas = new List<AreaModel>
            {
                AreaInstanceModel()
            };
            return areas;
        }
        #endregion

        #region Test Methods

        [Fact]
        public async void Delete_NonExistingArea_NotFound()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(nonExistingAreaId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await areaController.Delete(nonExistingAreaId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingArea_Ok()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(existingAreaId, CancellationToken.None))
                .ReturnsAsync(AreaInstance());

            _mockAreaRepository.Setup(ar => ar.Remove(existingAreaId))
                .Verifiable();

            // Act 
            var result = await areaController.Delete(existingAreaId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Get_NonExistingArea_NotFound()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(nonExistingAreaId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await areaController.Get(nonExistingAreaId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingArea_Ok()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(existingAreaId, CancellationToken.None))
                .ReturnsAsync(AreaInstance());

            // Act 
            var result = await areaController.Get(existingAreaId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 

            _mockAreaRepository.Setup(ar => ar.GetAllAsync<Area>(CancellationToken.None))
                .ReturnsAsync(AreaListInstance());

            // Act
            var result = await areaController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<AreaModel>>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void GetAreasFiltered_ModelError_BadRequest()
        {
            // Arrange
            areaController.ModelState.AddModelError("id", "id is null");

            // Act 
            var result = await areaController.GetAreasFiltered(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetAreasFiltered_ValidModel_Ok()
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

            _mockAreaRepository.Setup(ar => ar.GetAreasFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<AreaModel>>()
                {
                    Content = new PagedQueryResult<AreaModel>(
                        AreaListInstanceModel(),
                        searchModel.Take,
                        searchModel.Skip,
                        10)
                });

            // Act
            var result = await areaController.GetAreasFiltered(searchModel);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetAreasFiltered_DefaultTake_Ok()
        {
            // Arrange
            var searchModel = new PageDescriptor(null, null)
            {
                Take = 1000,
                Skip = 0,
            };

            var jsonSerializerMock = new Mock<IJsonSerializer>(MockBehavior.Strict);
            this.serviceProviderMock
                .Setup(provider => provider.GetService(typeof(IJsonSerializer)))
                .Returns(jsonSerializerMock.Object);

            _mockAreaRepository.Setup(ar => ar.GetAreasFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<AreaModel>>()
                {
                    Content = new PagedQueryResult<AreaModel>(
                        AreaListInstanceModel(),
                        500,
                        searchModel.Skip,
                        10)
                });

            // Act
            var result = await areaController.GetAreasFiltered(searchModel);

            // Assert
            // comprobar que si le pasamos mas de 500 en el take el objeto que devuelve tiene 500
            Assert.NotNull(result);
        }


        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            areaController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await areaController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ValidModel_Ok()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.Add(It.IsAny<Area>()))
                .ReturnsAsync(AreaInstance());


            // Act
            var result = await areaController.Post(AreaInstanceModel());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<AreaModel>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void Post_RepositoryException_BadRequest()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.ApplyChangesAsync(It.IsAny<Area>(), CancellationToken.None))
                .ThrowsAsync(new Exception());

            // Act
            var result = await areaController.Post(AreaInstanceModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PostWithAiepIDs_ErrorModel_BadRequest()
        {
            // Arrange
            areaController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await areaController.PostWithAiepIds(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PostWithAiepIDs_ValidModel_Ok()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.SaveArea(It.IsAny<AreaDtoModel>()))
                .ReturnsAsync(new RepositoryResponse<AreaModel>()
                {
                    Content = AreaInstanceModel()
                });

            // Act
            var result = await areaController.PostWithAiepIds(It.IsAny<AreaDtoModel>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<AreaModel>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            areaController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await areaController.Put(It.IsAny<int>(), It.IsAny<AreaModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingArea_NotFound()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.CheckIfExistsAsync(nonExistingAreaId))
                .ReturnsAsync(false);

            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(existingAreaId, CancellationToken.None))
               .ReturnsAsync(AreaInstance());

            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(nonExistingAreaId, CancellationToken.None))
               .ReturnsAsync(() => null);

            // Act
            var result = await areaController.Put(nonExistingAreaId, It.IsAny<AreaModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingArea_Ok()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.CheckIfExistsAsync(existingAreaId))
                .ReturnsAsync(true);

            _mockAreaRepository.Setup(ar => ar.ApplyChangesAsync(It.IsAny<Area>(), CancellationToken.None))
                .ReturnsAsync(AreaInstance());

            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(existingAreaId, CancellationToken.None))
                .ReturnsAsync(AreaInstance());

            // Act
            var result = await areaController.Put(existingAreaId, AreaInstanceModel());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Put_RepositoryException_BadRequest()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.CheckIfExistsAsync(existingAreaId))
                .ReturnsAsync(true);

            _mockAreaRepository.Setup(ar => ar.ApplyChangesAsync(It.IsAny<Area>(), CancellationToken.None))
                .ThrowsAsync(new Exception());

            _mockAreaRepository.Setup(ar => ar.FindOneAsync<Area>(existingAreaId, CancellationToken.None))
               .ReturnsAsync(AreaInstance());


            // Act
            var result = await areaController.Put(AreaInstance().Id, AreaInstanceModel());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PutWithAiepIDs_ErrorModel_BadRequest()
        {
            // Arrange
            areaController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await areaController.PutWithAiepIds(It.IsAny<int>(), It.IsAny<AreaDtoModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void PutWithAiepIDs_NonExistingArea_NotFound()
        {
            // Arrange
            _mockAreaRepository.Setup(ar => ar.CheckIfExistsAsync(nonExistingAreaId))
                .ReturnsAsync(false);

            // Act
            var result = await areaController.PutWithAiepIds(nonExistingAreaId, It.IsAny<AreaDtoModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void PutWithAiepIDs_ExistingArea_Ok()
        {
            // Arrange
            var model = new AreaDtoModel()
            {
                Id = existingAreaId
            };

            _mockAreaRepository.Setup(ar => ar.CheckIfExistsAsync(existingAreaId))
                .ReturnsAsync(true);

            _mockAreaRepository.Setup(ar => ar.SaveArea(model))
                .ReturnsAsync(new RepositoryResponse<AreaModel>()
                {
                    Content = AreaInstanceModel()
                });

            // Act
            var result = await areaController.PutWithAiepIds(AreaInstance().Id, model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion
    }
}
