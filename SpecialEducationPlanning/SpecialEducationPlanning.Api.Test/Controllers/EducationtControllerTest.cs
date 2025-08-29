using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

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
    public class AiepControllerTest : BaseTest
    {
        private readonly int existingAiepId = 1;
        private readonly int nonExistingAiepId = 99;

        private readonly int existingBuilderId = 1;

        private readonly Mock<IAiepRepository> _mockAiepRepository;
        private readonly Mock<IUserService> _mockUserService;

        private readonly AiepController AiepController;

        public AiepControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockAiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "2"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AiepModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            AiepController = new AiepController(
                _mockAiepRepository.Object,
                this.LoggerFactory.CreateLogger<AiepController>(),
                _mockUserService.Object,
                mapper.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };
        }

        #region Private Methods
        private AiepModel AiepInstance()
        {
            var model = new AiepModel
            {
                Id = existingAiepId
            };
            return model;
        }

        private ICollection<AiepModel> AiepListInstance()
        {
            ICollection<AiepModel> Aieps = new List<AiepModel>
            {
                AiepInstance()
            };
            return Aieps;
        }

        private Aiep AiepInstanceEntity()
        {
            var model = new Aiep
            {
                Id = existingAiepId
            };
            return model;
        }

        private ICollection<Aiep> AiepListInstanceEntity()
        {
            ICollection<Aiep> Aieps = new List<Aiep>
            {
                AiepInstanceEntity()
            };
            return Aieps;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingAiep_NotFound()
        {
            // Arrange
            _mockAiepRepository.Setup(dr => dr.FindOneAsync<Aiep>(nonExistingAiepId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await AiepController.Delete(nonExistingAiepId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingAiep_Ok()
        {
            // Arrange
            _mockAiepRepository.Setup(dr => dr.FindOneAsync<Aiep>(existingAiepId, CancellationToken.None))
                .ReturnsAsync(AiepInstanceEntity());

            _mockAiepRepository.Setup(dr => dr.Remove(existingAiepId))
                .Verifiable();

            // Act 
            var result = await AiepController.Delete(existingAiepId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Current Aiep Has Builder
        [Fact]
        public async void CurrentAiepHasBuilder_ExistingBuilder_Ok()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingAiepId);

            _mockAiepRepository.Setup(dr => dr.CheckBuilderInAiepAsync(existingAiepId, existingBuilderId))
                .ReturnsAsync(new RepositoryResponse<bool>() { Content = true });

            // Act
            var result = await AiepController.CurrentAiepHasBuilder(existingBuilderId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get by ID
        [Fact]
        public async void Get_NonExistingAiep_NotFound()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.FindOneAsync<Aiep>(nonExistingAiepId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await AiepController.Get(nonExistingAiepId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingAiep_Ok()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.FindOneAsync<Aiep>(existingAiepId, CancellationToken.None))
                .ReturnsAsync(AiepInstanceEntity());

            // Act
            var result = await AiepController.Get(existingAiepId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.GetAllAsync<Aiep>(CancellationToken.None))
                .ReturnsAsync(AiepListInstanceEntity());

            // Act
            var result = await AiepController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<AiepModel>>((result as OkObjectResult).Value);
        }
        #endregion

        // To do
        #region Get All Aieps by Area

        #endregion

        // To do
        #region Get Aieps Filtered

        #endregion

        #endregion

        #region Post

        #region Post by Model
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            AiepController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await AiepController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ValidModel_Ok()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.CreateUpdateAiepAsync(It.IsAny<AiepModel>()))
                .ReturnsAsync(AiepInstance());

            // Act
            var result = await AiepController.Post(AiepInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<AiepModel>((result as OkObjectResult).Value);
        }
        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            AiepController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await AiepController.Put(It.IsAny<int>(), It.IsAny<AiepModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingAiep_NotFound()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.CheckIfExistsAsync(nonExistingAiepId))
                .ReturnsAsync(false);

            // Act  
            var result = await AiepController.Put(nonExistingAiepId, It.IsAny<AiepModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingAiep_Ok()
        {
            // Arrange
            _mockAiepRepository.Setup(ar => ar.CheckIfExistsAsync(existingAiepId))
                .ReturnsAsync(true);

            _mockAiepRepository.Setup(ar => ar.CreateUpdateAiepAsync(It.IsAny<AiepModel>()))
                .ReturnsAsync(AiepInstance());

            // Act
            var result = await AiepController.Put(existingAiepId, AiepInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion

        #endregion
    }
}
