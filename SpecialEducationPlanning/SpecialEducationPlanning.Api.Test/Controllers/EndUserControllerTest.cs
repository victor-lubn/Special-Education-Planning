using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
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
using Xunit;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Xunit.Abstractions;
using System.Threading;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class EndUserControllerTest : BaseTest
    {
        private readonly int existingEndUserId = 1;
        private readonly int nonExistingEndUserId = 99;
        private readonly int existingAiepId = 1;
        private readonly int nonExistingAiepId = 0;

        private readonly string validPostCode = "PE26 1DQ";
        private readonly string validFormatPostCode = "PE261DQ";
        private readonly string nonValidPostCode = "XXX XXX";
        private readonly string validSurname = "Surname";
        private readonly string invalidSurname = "Invalid Surname";
        private readonly string validAddress1 = "Address 1";
        private readonly string invalidAddress1 = "Invalid Address 1";

        private readonly IBuilderRepository builderRepository;
        private readonly IPlanRepository planRepository;
        private readonly IEndUserRepository repository;
        private readonly IActionRepository actionRepository;
        private readonly IUserService userService;
        private readonly ILogger<EndUserController> logger;
        private readonly IPostCodeServiceFactory postCodeServiceFactory;

        private readonly Mock<IEndUserRepository> _mockEndUserRepository;
        private readonly Mock<IBuilderRepository> _mockBuilderRepository;
        private readonly Mock<IPlanRepository> _mockPlanRepository;
        private readonly Mock<IActionRepository> _mockActionRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IPostCodeServiceFactory> _mockPostCodeServiceFactory;

        private readonly EndUserController endUserController;

        public EndUserControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockEndUserRepository = new Mock<IEndUserRepository>(MockBehavior.Default);
            _mockBuilderRepository = new Mock<IBuilderRepository>(MockBehavior.Strict);
            _mockPlanRepository = new Mock<IPlanRepository>(MockBehavior.Strict);
            _mockActionRepository = new Mock<IActionRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            _mockPostCodeServiceFactory = new Mock<IPostCodeServiceFactory>(MockBehavior.Strict);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(EndUserModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());


            endUserController = new EndUserController(
                _mockEndUserRepository.Object,
                _mockBuilderRepository.Object,
                _mockPlanRepository.Object,
                _mockActionRepository.Object,
                _mockUserService.Object,
                this.LoggerFactory.CreateLogger<EndUserController>(),
                _mockPostCodeServiceFactory.Object,
                mapper.Object
            )
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user
                    }
                }
            };
        }

        #region Private Methods
        private EndUserModel EndUserInstance()
        {
            var model = new EndUserModel
            {
                Id = existingEndUserId,
                Postcode = validPostCode,
                Surname = validSurname,
                Address1 = validAddress1
            };
            return model;
        }

        private EndUser EndUserInstanceEntity()
        {
            var model = new EndUser
            {
                Id = existingEndUserId,
                Postcode = validPostCode,
                Surname = validSurname,
                Address1 = validAddress1
            };
            return model;
        }

        private EndUserModel InvalidEndUserInstance()
        {
            var model = new EndUserModel
            {
                Id = nonExistingEndUserId,
                Postcode = nonValidPostCode,
                Surname = invalidSurname,
                Address1 = invalidAddress1
            };
            return model;
        }

        private ICollection<EndUserModel> EndUserListInstance()
        {
            ICollection<EndUserModel> endUsers = new List<EndUserModel>
            {
                EndUserInstance()
            };
            return endUsers;
        }

        private AiepModel AiepInstance()
        {
            var model = new AiepModel
            {
                Id = existingAiepId
            };
            return model;
        }
        #endregion

        #region Test Methods
        #region Delete
        [Fact]
        public async void Delete_NonExistingEndUser_NotFound()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.FindOneAsync<EndUser>(nonExistingEndUserId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await endUserController.Delete(nonExistingEndUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingEndUser_Ok()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.FindOneAsync<EndUser>(existingEndUserId, CancellationToken.None))
                .ReturnsAsync(EndUserInstanceEntity());

            _mockEndUserRepository.Setup(eur => eur.Remove(existingEndUserId))
                .Verifiable();

            // Act 
            var result = await endUserController.Delete(existingEndUserId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void EndUserRightToBeForgottenTwoYearAfterAsync_ErrorAnonymising()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.EndUserCleanManagment())
                .ReturnsAsync(new RepositoryResponseGeneric() { Content = null });

            // Act
            var result = await endUserController.EndUserRightToBeForgottenTwoYearAfterAsync();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void EndUserRightToBeForgottenTwoYearAfterAsync_Ok()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.EndUserCleanManagment())
                .ReturnsAsync(new RepositoryResponseGeneric() { Content = "" });

            // Act
            var result = await endUserController.EndUserRightToBeForgottenTwoYearAfterAsync();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get
        [Fact]
        public async void Get_NonExistingEndUser_NotFound()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.GetEndUserById(nonExistingEndUserId))
                .ReturnsAsync(new RepositoryResponse<EndUserModel> { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

            // Act
            var result = await endUserController.Get(nonExistingEndUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingEndUser_Ok()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.GetEndUserById(existingEndUserId))
                .ReturnsAsync(new RepositoryResponse<EndUserModel> { Content = EndUserInstance() });

            // Act 
            var result = await endUserController.Get(existingEndUserId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetEndUserGetLatestAiep_ModelError_BadRequest()
        {
            // Arrange
            endUserController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await endUserController.GetEndUserGetLatestAiep(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetEndUserGetLatestAiep_NoAiep_BadRequest()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(nonExistingAiepId);

            // Act
            var result = await endUserController.GetEndUserGetLatestAiep(EndUserInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetEndUserGetLatestAiep_AiepOk_PostCodeError_BadRequest()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingAiepId);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(nonValidPostCode))
                .Returns(new RepositoryResponse<string> { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

            // Act
            var result = await endUserController.GetEndUserGetLatestAiep(InvalidEndUserInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetEndUserGetLatestAiep_AiepOk_NoPostCode_BadRequest()
        {
            // Arrange
            _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
                .Returns(existingAiepId);

            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(nonValidPostCode))
                .Returns(new RepositoryResponse<string> { Content = null });

            // Act
            var result = await endUserController.GetEndUserGetLatestAiep(InvalidEndUserInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        //[Fact]
        //public async void GetEndUserGetLatestAiep_AiepOk_PostCodeOk_NoEndUserMatch_BadRequest()
        //{
        //    // Arrange
        //    _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
        //        .Returns(existingAiepId);

        //    _mockPostCodeService.Setup(pcs => pcs.GetPostCode(validPostCode))
        //        .Returns(new RepositoryResponse<string> { Content = validFormatPostCode });

        //    _mockEndUserRepository.Setup(eur => eur.GetEndUserByMandatoryFieldsAsync(EndUserInstance()))
        //        .ReturnsAsync(new RepositoryResponse<EndUserModel> { Content = null });

        //    // Act
        //    var result = await endUserController.GetEndUserGetLatestAiep(EndUserInstance());

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async void GetEndUserGetLatestAiep_AiepOk_PostCodeOk_EndUserMatchOK_NoLatestAiep_BadRequest()
        //{
        //    // Arrange
        //    _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
        //        .Returns(existingAiepId);

        //    _mockPostCodeService.Setup(pcs => pcs.GetPostCode(validPostCode))
        //        .Returns(new RepositoryResponse<string> { Content = validFormatPostCode });

        //    _mockEndUserRepository.Setup(eur => eur.GetEndUserByMandatoryFieldsAsync(EndUserInstance()))
        //        .ReturnsAsync(new RepositoryResponse<EndUserModel> { Content = EndUserInstance() });

        //    _mockEndUserRepository.Setup(eur => eur.GetEndUserOwnOrLatestUserAiepAsync(existingEndUserId, nonExistingAiepId))
        //        .ReturnsAsync(new RepositoryResponse<AiepModel> { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

        //    // Act
        //    var result = await endUserController.GetEndUserGetLatestAiep(EndUserInstance());

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async void GetEndUserGetLatestAiep_Ok()
        //{
        //    // Arrange
        //    _mockUserService.Setup(us => us.GetUserAiepId(It.IsAny<ClaimsIdentity>()))
        //        .Returns(existingAiepId);

        //    _mockPostCodeService.Setup(pcs => pcs.GetPostCode(validPostCode))
        //        .Returns(new RepositoryResponse<string> { Content = validFormatPostCode });

        //    _mockEndUserRepository.Setup(eur => eur.GetEndUserByMandatoryFieldsAsync(EndUserInstance()))
        //        .ReturnsAsync(new RepositoryResponse<EndUserModel> { Content = EndUserInstance() });

        //    _mockEndUserRepository.Setup(eur => eur.GetEndUserOwnOrLatestUserAiepAsync(existingEndUserId, existingAiepId))
        //        .ReturnsAsync(new RepositoryResponse<AiepModel> { Content = AiepInstance() });

        //    // Act
        //    var result = await endUserController.GetEndUserGetLatestAiep(EndUserInstance());

        //    // Assert
        //    Assert.IsType<OkObjectResult>(result);
        //}
        #endregion

        #region Post
        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            endUserController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await endUserController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ModelOk_PostCodeError_BadRequest()
        {
            // Arrange
            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(nonValidPostCode))
                .Returns(new RepositoryResponse<string> { ErrorList = { ErrorCode.EntityNotFound.GetDescription() } });

            // Act
            var result = await endUserController.Post(InvalidEndUserInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ModelOk_NoPostCode_BadRequest()
        {
            // Arrange
            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(nonValidPostCode))
                .Returns(new RepositoryResponse<string> { Content = null });

            // Act
            var result = await endUserController.Post(InvalidEndUserInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Post_ModelOk_PostCode_Success()
        {
            // Arrange
            _mockPostCodeServiceFactory.Setup(pcs => pcs.GetService(null).GetPostCode(validPostCode))
                .Returns(new RepositoryResponse<string> { Content = validFormatPostCode });

            _mockEndUserRepository.Setup(eur => eur.ApplyChangesAsync(It.IsAny<EndUser>(), CancellationToken.None))
                .Returns(Task.FromResult(EndUserInstanceEntity()));

            _mockEndUserRepository.Setup(r => r.Add(It.IsAny<EndUser>()))
                .ReturnsAsync(EndUserInstanceEntity);


            // Act
            var result = await endUserController.Post(EndUserInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<EndUserModel>((result as OkObjectResult).Value);
        }
        #endregion

        #region Put
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            endUserController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await endUserController.Put(0, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NoOlderEndUser_NotFound()
        {
            // Arrange
            _mockEndUserRepository.Setup(eur => eur.FindOneAsync<EndUser>(nonExistingEndUserId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await endUserController.Put(nonExistingEndUserId, InvalidEndUserInstance());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        //[Fact]
        //public async void Put_OlderEndUser_NoPostCode_BadRequest()
        //{
        //    // Arrange
        //    _mockEndUserRepository.Setup(eur => eur.FindOneAsync<EndUserModel>(existingEndUserId))
        //        .ReturnsAsync(() => EndUserInstance());

        //    _mockPostCodeService.Setup(pcs => pcs.GetPostCode(nonValidPostCode))
        //        .Returns(new RepositoryResponse<string> { Content = null });

        //    // Act
        //    var result = await endUserController.Put(existingEndUserId, EndUserInstance());

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async void Put_OlderEndUser_PostCode_BadRequest()
        //{
        //    // Arrange
        //    _mockEndUserRepository.Setup(eur => eur.FindOneAsync<EndUserModel>(existingEndUserId))
        //        .ReturnsAsync(() => EndUserInstance());

        //    _mockPostCodeService.Setup(pcs => pcs.GetPostCode(validPostCode))
        //         .Returns(new RepositoryResponse<string> { Content = validFormatPostCode });

        //    _mockEndUserRepository.Setup(eur => eur.ApplyChangesAsync(It.IsAny<EndUserModel>()))
        //        .Returns(Task.FromResult(EndUserInstance()));

        //    // Act
        //    var result = await endUserController.Put(existingEndUserId, EndUserInstance());

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}
        #endregion
        #endregion
    }
}
