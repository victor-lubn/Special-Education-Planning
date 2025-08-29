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
    public class EducationerControllerTest : BaseTest
    {
        private readonly int existingEducationerId = 1;
        private readonly int nonExistingEducationerId = 99;

        private const int foundId = 1;
        private const int notFoundId = 2;
        private const int foundWithLastVersion = 3;
        private const int releaseVersion = 1;
        private const int foundEducationer = 1;

        private readonly Mock<IEducationerRepository> _mockEducationerRepository;
        private readonly Mock<IBuilderRepository> _mockBuilderRepository;
        private readonly Mock<IAiepRepository> _mockAiepRepository;
        private readonly Mock<IUserService> _mockUserService;

        private readonly EducationerController EducationerController;

        public EducationerControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockEducationerRepository = new Mock<IEducationerRepository>(MockBehavior.Strict);
            _mockBuilderRepository = new Mock<IBuilderRepository>(MockBehavior.Strict);
            _mockAiepRepository = new Mock<IAiepRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(UserModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            EducationerController = new EducationerController(
                _mockEducationerRepository.Object,
                _mockBuilderRepository.Object,
                _mockAiepRepository.Object,
                _mockUserService.Object,
                this.LoggerFactory.CreateLogger<EducationerController>(),
                mapper.Object
            );
        }

        #region Private Methods
        private UserModel EducationerInstance()
        {
            var model = new UserModel
            {
                Id = existingEducationerId
            };
            return model;
        }

        private ICollection<UserModel> EducationerListInstance()
        {
            ICollection<UserModel> Educationers = new List<UserModel>
            {
                EducationerInstance()
            };
            return Educationers;
        }

        private User EducationerInstanceEntity()
        {
            var model = new User
            {
                Id = existingEducationerId
            };
            return model;
        }

        private ICollection<User> EducationerListInstanceEntity()
        {
            ICollection<User> Educationers = new List<User>
            {
                EducationerInstanceEntity()
            };
            return Educationers;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingEducationer_NotFound()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(nonExistingEducationerId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act  
            var result = await EducationerController.Delete(nonExistingEducationerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingEducationer_Ok()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(existingEducationerId, CancellationToken.None))
                .ReturnsAsync(EducationerInstanceEntity());

            _mockEducationerRepository.Setup(dr => dr.Remove(existingEducationerId))
                .Verifiable();

            // Act 
            var result = await EducationerController.Delete(existingEducationerId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingEducationer_NotFound()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(nonExistingEducationerId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await EducationerController.Get(nonExistingEducationerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingEducationer_Ok()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(existingEducationerId, CancellationToken.None))
                .ReturnsAsync(EducationerInstanceEntity());

            // Act
            var result = await EducationerController.Get(existingEducationerId);


            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.GetAllAsync<User>(CancellationToken.None))
                .ReturnsAsync(EducationerListInstanceEntity());

            // Act
            var result = await EducationerController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<UserModel>>((result as OkObjectResult).Value);
        }
        #endregion

        #region Get Release Info
        [Fact]
        public async void GetReleaseInfo_Ok()
        {
            // Arrange
            EducationerController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal()
                }
            };

            _mockUserService.Setup(us => us.GetUserId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingEducationerId);

            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(existingEducationerId, CancellationToken.None))
                .ReturnsAsync(EducationerInstanceEntity());

            // Act
            var result = await this.EducationerController.GetReleaseInfo();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion

        #region Post

        #region Post by Model
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            EducationerController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await EducationerController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ValidModel_Ok()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.Add(It.IsAny<User>()))
                .ReturnsAsync(EducationerInstanceEntity());

            // Act
            var result = await EducationerController.Post(EducationerInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserModel>((result as OkObjectResult).Value);
        }
        #endregion

        // To do
        #region Set Release Info

        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            EducationerController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await EducationerController.Put(It.IsAny<int>(), It.IsAny<UserModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingEducationer_NotFound()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.CheckIfExistsAsync(nonExistingEducationerId))
                .ReturnsAsync(false);

            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(nonExistingEducationerId, CancellationToken.None))
                .ReturnsAsync(() => null);
            // Act  
            var result = await EducationerController.Put(nonExistingEducationerId, It.IsAny<UserModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingEducationer_Ok()
        {
            // Arrange
            _mockEducationerRepository.Setup(dr => dr.CheckIfExistsAsync(existingEducationerId))
                .ReturnsAsync(true);

            _mockEducationerRepository.Setup(dr => dr.Add(It.IsAny<User>()))
                .ReturnsAsync(EducationerInstanceEntity());

            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(nonExistingEducationerId, CancellationToken.None))
                .ReturnsAsync(() => null);

            _mockEducationerRepository.Setup(dr => dr.CheckIfExistsAsync(existingEducationerId))
                .ReturnsAsync(true);

            _mockEducationerRepository.Setup(dr => dr.FindOneAsync<User>(existingEducationerId, CancellationToken.None))
                .ReturnsAsync(EducationerInstanceEntity());

            _mockEducationerRepository.Setup(dr => dr.Add(It.IsAny<User>()))
                .ReturnsAsync(EducationerInstanceEntity());

            _mockEducationerRepository.Setup(cr => cr.ApplyChangesAsync(It.IsAny<User>(), CancellationToken.None))
                .ReturnsAsync(EducationerInstanceEntity());
   

            // Act  
            var result = await EducationerController.Put(existingEducationerId, EducationerInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion

        #endregion
    }
}

