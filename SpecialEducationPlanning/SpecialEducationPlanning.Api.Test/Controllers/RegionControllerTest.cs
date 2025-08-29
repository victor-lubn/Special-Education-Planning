using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

using Moq;
using Xunit;
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
    public class RegionControllerTest : BaseTest
    {
        private readonly int existingRegionId = 1;
        private readonly int nonExistingRegionId = 99;

        private readonly Mock<IRegionRepository> _mockRegionRepository;

        private readonly RegionController _regionController;
        public RegionControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockRegionRepository = new Mock<IRegionRepository>(MockBehavior.Strict);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(RegionModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            _regionController = new RegionController(
                _mockRegionRepository.Object,
                this.LoggerFactory.CreateLogger<RegionController>(),
                mapper.Object
            );
        }

        #region Private Methods
        private RegionModel RegionExistingInstance()
        {
            var model = new RegionModel
            {
                Id = existingRegionId
            };
            return model;
        }

        private ICollection<RegionModel> RegionListInstance()
        {
            ICollection<RegionModel> regions = new List<RegionModel>
            {
                RegionExistingInstance()
            };
            return regions;
        }

        private Region RegionExistingInstanceEntity()
        {
            var entity = new Region
            {
                Id = existingRegionId
            };
            return entity;
        }

        private ICollection<Region> RegionListInstanceEntity()
        {
            ICollection<Region> regions = new List<Region>
            {
                RegionExistingInstanceEntity()
            };
            return regions;
        }
        #endregion

        #region Test Methods

        #region Delete
        [Fact]
        public async void Delete_NonExistingRegion_NotFound()
        {
            // Arrange
            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(nonExistingRegionId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await _regionController.Delete(nonExistingRegionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingRegion_Ok()
        {
            // Arrange
            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(existingRegionId, CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());

            _mockRegionRepository.Setup(rr => rr.Remove(existingRegionId))
                .Verifiable();

            // Act 
            var result = await _regionController.Delete(existingRegionId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingArea_NotFound()
        {
            // Arrange
            _mockRegionRepository.Setup(m => m.FindOneAsync<Region>(nonExistingRegionId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await _regionController.Get(nonExistingRegionId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingRegion_Ok()
        {
            //Arrange
            _mockRegionRepository.Setup(m => m.FindOneAsync<Region>(existingRegionId, CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());

            // Act
            var result = await _regionController.Get(existingRegionId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 
            _mockRegionRepository.Setup(rr => rr.GetAllAsync<Region>(CancellationToken.None))
                .ReturnsAsync(RegionListInstanceEntity());

            // Act
            var result = await _regionController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<RegionModel>>((result as OkObjectResult).Value);
        }
        #endregion

        // To do
        #region Get With Childrens

        #endregion

        // To do
        #region Get Regions Filtered

        #endregion

        #endregion

        #region Post
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            _regionController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await _regionController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ValidModel_Ok()
        {
            //Arrange

            // Arrange
            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(existingRegionId, CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());

            _mockRegionRepository.Setup(rr => rr.GetDuplicatedRegion(It.IsAny<RegionModel>()))
                .ReturnsAsync(new RepositoryResponse<RegionModel>(RegionExistingInstance()));

            _mockRegionRepository.Setup(rr => rr.ApplyChangesAsync(It.IsAny<Region>(), CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());

            //Act
            var result = await _regionController.Post(RegionExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<RegionModel>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void Post_DuplicatedRegion_BadRequest()
        {
            //Arrange
            _mockRegionRepository.Setup(rr => rr.GetDuplicatedRegion(It.IsAny<RegionModel>()))
                .ReturnsAsync(new RepositoryResponse<RegionModel>(null, ErrorCode.EntityAlreadyExist));

            // Act
            var result = await _regionController.Post(RegionExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion

        #region Put
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            _regionController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await _regionController.Put(It.IsAny<int>(), It.IsAny<RegionModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingRegion_BadRequest()
        {
            // Arrange
            _mockRegionRepository.Setup(rr => rr.CheckIfExistsAsync(existingRegionId))
                .ReturnsAsync(false);

            _mockRegionRepository.Setup(repo => repo.GetDuplicatedRegion(It.IsAny<RegionModel>()))
                .ReturnsAsync(new RepositoryResponse<RegionModel>(new RegionModel()));
            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken token) => null);


            // Act
            var result = await _regionController.Put(RegionExistingInstance().Id, RegionExistingInstance());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_DuplicatedRegion_BadRequest()
        {
            //Arrange
            _mockRegionRepository.Setup(rr => rr.CheckIfExistsAsync(existingRegionId))
                .ReturnsAsync(true);

            _mockRegionRepository.Setup(rr => rr.GetDuplicatedRegion(It.IsAny<RegionModel>()))
                .ReturnsAsync(new RepositoryResponse<RegionModel>(null, ErrorCode.EntityAlreadyExist));

            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(existingRegionId, CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());



            // Act
            var result = await _regionController.Put(RegionExistingInstance().Id, RegionExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_ExistingRegion_Ok()
        {
            // Arrange

            _mockRegionRepository.Setup(rr => rr.FindOneAsync<Region>(existingRegionId, CancellationToken.None))
    .ReturnsAsync(RegionExistingInstanceEntity());

            _mockRegionRepository.Setup(rr => rr.CheckIfExistsAsync(existingRegionId))
                .ReturnsAsync(true);

            _mockRegionRepository.Setup(rr => rr.GetDuplicatedRegion(It.IsAny<RegionModel>()))
                .ReturnsAsync(new RepositoryResponse<RegionModel>(RegionExistingInstance()));

            _mockRegionRepository.Setup(rr => rr.ApplyChangesAsync(It.IsAny<Region>(), CancellationToken.None))
                .ReturnsAsync(RegionExistingInstanceEntity());

            // Act
            var result = await _regionController.Put(existingRegionId, RegionExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion
    }
}