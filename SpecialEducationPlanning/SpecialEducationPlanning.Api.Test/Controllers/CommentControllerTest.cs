using System.Security.Claims;
using System.Collections.Generic;

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
    public class CommentControllerTest : BaseTest
    {
        private readonly int existingCommentId = 1;
        private readonly int nonExistingCommentId = 99;

        private readonly string userUniqueIdentifier = "user@aiep.com";

        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IUserService> _mockUserService;

        private readonly CommentController commentController;

        public CommentControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockCommentRepository = new Mock<ICommentRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(CommentModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            commentController = new CommentController(
                _mockCommentRepository.Object,
                this.LoggerFactory.CreateLogger<CommentController>(),
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
        private CommentModel CommentInstance()
        {
            var model = new CommentModel()
            {
                Id = existingCommentId
            };
            return model;
        }
        private Comment CommentInstanceEntity()
        {
            var model = new Comment()
            {
                Id = existingCommentId
            };
            return model;
        }

        private ICollection<CommentModel> CatalogListInstance()
        {
            ICollection<CommentModel> comments = new List<CommentModel>
            {
                CommentInstance()
            };
            return comments;
        }
        #endregion

        #region Test Methods

        #region Delete

        #region Delete by ID
        [Fact]
        public async void Delete_NonExistingComment_NotFound()
        {
            // Assert
            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(nonExistingCommentId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await commentController.Delete(nonExistingCommentId);

            // Arange
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_NotAuthorizedUser_Unathorized()
        {
            // Assert
            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(existingCommentId, CancellationToken.None))
                .ReturnsAsync(CommentInstanceEntity());

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userUniqueIdentifier);
            _mockUserService.Setup(us => us.GetUserPermissions(It.IsAny<ClaimsIdentity>()))
                .Returns(new List<Claim>());

            // Act
            var result = await commentController.Delete(existingCommentId);

            // Arrange
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void Delete_ExistingComment_Ok()
        {
            // Assert
            var model = CommentInstanceEntity();
            model.User = userUniqueIdentifier;

            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(existingCommentId, CancellationToken.None))
                .ReturnsAsync(model);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userUniqueIdentifier);

            _mockCommentRepository.Setup(cr => cr.Remove(existingCommentId))
                .Verifiable();

            // Act
            var result = await commentController.Delete(existingCommentId);

            // Arrange
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #region Put

        #region Put by Model
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            commentController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await commentController.Put(It.IsAny<int>(), It.IsAny<CommentModel>());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingComment_NotFound()
        {
            // Arrange
            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(nonExistingCommentId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await commentController.Put(nonExistingCommentId, It.IsAny<CommentModel>());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingComment_Ok()
        {
            // Assert
            var entity = CommentInstanceEntity();
            entity.User = userUniqueIdentifier;

            var model = CommentInstance();
            model.User = userUniqueIdentifier;

            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(existingCommentId, CancellationToken.None))
                .ReturnsAsync(entity);

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userUniqueIdentifier);

            _mockCommentRepository.Setup(cr => cr.UpdateComment<PlanModel>(model, userUniqueIdentifier))
                .ReturnsAsync(new RepositoryResponse<CommentModel>());

            // Act
            var result = await commentController.Put(existingCommentId, model);

            // Arrange
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Put_NotAuthorizedUser_Unathorized()
        {
            // Assert
            _mockCommentRepository.Setup(cr => cr.FindOneAsync<Comment>(existingCommentId, CancellationToken.None))
                .ReturnsAsync(CommentInstanceEntity());

            _mockUserService.Setup(us => us.GetUserIdentifier(It.IsAny<ClaimsIdentity>()))
                .Returns(userUniqueIdentifier);
            _mockUserService.Setup(us => us.GetUserPermissions(It.IsAny<ClaimsIdentity>()))
                .Returns(new List<Claim>());

            // Act
            var result = await commentController.Put(existingCommentId, It.IsAny<CommentModel>());

            // Arrange
            Assert.IsType<UnauthorizedResult>(result);
        }
        #endregion

        #endregion

        #endregion
    }
}
