using Koa.Hosting.AspNetCore.Model;
using Koa.Persistence.Abstractions.QueryResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.UserInfoModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Api.Test.Support;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class UserControllerTests : BaseTest
    {
        private readonly Mock<IUserService> mockUserService;
        private readonly Mock<IUserRepository> mockUserRepository;
        private readonly Mock<IAiepRepository> mockAiepRepository;
        private readonly Mock<IEducationerRepository> mockEducationerRepository;
        private readonly Mock<ILogger<UserController>> mockLogger;
        private readonly Mock<IFeatureManagementService> _mockFeatureManagementService;
        private readonly UserController controller;

        public UserControllerTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) :base(fixture, outputHelper)
        {
            mockUserService = new Mock<IUserService>();
            mockUserRepository = new Mock<IUserRepository>();
            mockAiepRepository = new Mock<IAiepRepository>();
            mockEducationerRepository = new Mock<IEducationerRepository>();
            _mockFeatureManagementService = new Mock<IFeatureManagementService>();

            _mockFeatureManagementService.Setup(a => a.GetFeatureFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(false);

            var config = new OptionsManager<UserInfoUrlConfiguration>(new OptionsFactory<UserInfoUrlConfiguration>(
                new List<IConfigureOptions<UserInfoUrlConfiguration>>(),
                new List<IPostConfigureOptions<UserInfoUrlConfiguration>>()));
            mockLogger = new Mock<ILogger<UserController>>();

            var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(UserModelProfile));
                cfg.AddProfile(typeof(AiepModelProfile));

            });
            var mapper = new Mock<AutoMapperObjectMapper>(mapperConfig.CreateMapper());

            controller = new UserController(
                mockUserService.Object,
                mockUserRepository.Object,
                mockAiepRepository.Object,
                mockEducationerRepository.Object,
                config,
                mockLogger.Object,
                mapper.Object,
                _mockFeatureManagementService.Object);
        }

        [Fact]
        public async void Delete_IfRepositoryResponseHasErrors_ReturnsBadRequestResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.DeleteUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<bool>());

            //Act.
            var result = await controller.Delete(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void Delete_IfRepositoryResponseHasNoErrors_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.DeleteUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<bool> { Content = true });

            //Act.
            var result = await controller.Delete(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void Get_IfRepositoryResponseContentIsNull_ReturnsNotFoundResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockUserRepository.Setup(rep => rep.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserModel>());

            //Act.
            var result = await controller.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "UserController called Get", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "No user found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "UserController end call Get -> return Not found", times);
        }

        [Fact]
        public async void Get_IfRepositoryResponseContentIsNotNull_ReturnsOkObjectResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserModel> { Content = new UserModel() });

            //Act.
            var result = await controller.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<UserModel>(response.Value);
        }

        [Fact]
        public async void GetAllUsersByRoleId_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetAllUsersByRoleId(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserModel>>());

            //Act.
            var result = await controller.GetAllUsersByRoleId(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void GetAllUsersWithRoles_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetAllUsersWithRolesAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserWithRoleModel>>());

            //Act.
            var result = await controller.GetAllUsersWithRoles();

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void GetAllUsersWithRolesAndPermissions_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetAllUsersWithRolesAndPermissionsAsync())
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserWithRoleAndPermissionsModel>>());

            //Act.
            var result = await controller.GetAllUsersWithRolesAndPermissions();

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void GetCurrentUserInfo_IfUserRepositoryResponseHasError_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(
                new RepositoryResponse<UserModel>
                {
                    ErrorList = new List<string>
                        {"Test Error"}
                });

            //Act.
            var result = await controller.GetCurrentUserInfo();

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("Test Error", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "UserController called GetCurrentUserInfo", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "Errors getting current user info Test Error", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "UserController end call GetCurrentUserInfo -> return Errors", times);
        }

        [Fact]
        public async void GetCurrentUserInfo_IfNoAccessToAssignedAiep_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserModel>
                {
                    Content = new UserModel
                    {
                        Id = 1
                    }
                });

            mockEducationerRepository.Setup(x => x.GetPendingReleaseInfo(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<int?>
                {
                    ErrorList = new List<string>
                        {"Test Error"}
                });

            //Act.
            var result = await controller.GetCurrentUserInfo();

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal("No access to assigned Aiep", value);
        }

        [Fact]
        public async void GetCurrentUserInfo_IfCurrentAiepIdIsValid_ReturnsOkObjectResult()
        {
            //Arrange.
            mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserModel>
                {
                    Content = new UserModel
                    {
                        Id = 1
                    }
                });

            mockEducationerRepository.Setup(x => x.GetPendingReleaseInfo(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<int?>());
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var webUser = (ClaimsIdentity)controller.User.Identity;
            mockUserService.Setup(s => s.GetUserAssignedAiepId(webUser))
                .Returns(1);

            mockUserService.Setup(s => s.GetUserCurrentAiepId(webUser))
                .Returns(2);

            mockAiepRepository.Setup(rep => rep.FindOneAsync<Aiep>(1, System.Threading.CancellationToken.None))
                .ReturnsAsync(new Aiep { Id = 555, AiepCode = "Aiep555" });
            mockAiepRepository.Setup(rep => rep.FindOneAsync<Aiep>(1, System.Threading.CancellationToken.None))
                .ReturnsAsync(new Aiep { Id = 333, AiepCode = "Aiep333" });

            //Act.
            var result = await controller.GetCurrentUserInfo();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<UserInfoModel>(response.Value);
            Assert.Equal("Aiep333", value.AiepCode);
        }

        [Fact]
        public async void GetUsersWithRolesFiltered_IfModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            controller.ModelState.AddModelError("Key", "TestError");

            //Act.
            var result = await controller.GetUsersWithRolesFiltered(It.IsAny<Koa.Domain.Search.Page.PageDescriptor>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
        }

        [Fact]
        public async void GetUsersWithRolesFiltered_IfUsersWithRolesModelsHasErrors_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var searchModel = new Koa.Domain.Search.Page.PageDescriptor();
            this.mockLogger.MockLog(LogLevel.Error);
            Times? times = Times.Once();

            mockUserRepository.Setup(rep => rep.GetUsersWithRolesFilteredAsync(searchModel))
                .ReturnsAsync(new RepositoryResponse<IPagedQueryResult<UserWithRoleModel>>
                {
                    ErrorList = new List<string> { "TestError" }
                });

            //Act.
            var result = await controller.GetUsersWithRolesFiltered(searchModel);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);

            this.mockLogger.VerifyLogger(LogLevel.Error, "Error found: TestError", times);
        }

        [Fact]
        public async void GetUserWithRoles_IfUserIsAbsent_ReturnsNotFoundResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetUserWithRolesAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserWithRoleModel>());

            //Act.
            var result = await controller.GetUserWithRoles(It.IsAny<int>());

            //Arrange.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async void GetUserWithRoles_IfUserExists_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep => rep.GetUserWithRolesAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserWithRoleModel>
                {
                    Content = new UserWithRoleModel { Id = 7 }
                });

            //Act.
            var result = await controller.GetUserWithRoles(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<UserWithRoleModel>(response.Value);
            Assert.Equal(7, value.Id);
        }

        [Fact]
        public async void Post_IfModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            controller.ModelState.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Error);
            Times? times = Times.Once();

            //Act.
            var result = await controller.Post(It.IsAny<UserModel>(), It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);

            mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
        }

        [Fact]
        public async void Post_IfModelStateIsValid_ReturnsOkResult()
        {
            //Arrange.
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            mockUserRepository.Setup(rep =>
                    rep.CreateUserWithRole(It.IsAny<UserModel>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<UserModel>());

            //Act.
            var result = await controller.Post(It.IsAny<UserModel>(), It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void PostUsersFromCsv_IfFileToUploadIsNull_ReturnsBadRequestResult()
        {
            //Arrange.

            //Act.
            var result = await controller.PostUsersFromCsv(null);

            //Assert.
            var response = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async void PostUsersFromCsv_IfFileToUploadIsNotNull_ReturnsOkResult()
        {
            //Arrange.
            mockUserRepository.Setup(rep =>
                    rep.CreateUserFromCsvModel(new List<UserCsvModel>()))
                .ReturnsAsync(new RepositoryResponse<int> { Content = 1 });

            var fileUpload = new MultiUploadedFileModel
            {
                Files = new List<StreamFileModel>
                {
                    new StreamFileModel(
                        new MemoryStream(),
                        "FileName",
                        "FileContentType")
                }
            };

            //Act.
            var result = await controller.PostUsersFromCsv(fileUpload);

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<int>(response.Value);
            Assert.Equal(1, value);
        }

        [Fact]
        public async void Put_IfModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            controller.ModelState.AddModelError("Key", "TestError");

            //Act.
            var result = await controller.Put(
                It.IsAny<UserModel>(),
                It.IsAny<int>(),
                It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);

            mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
        }

        [Fact]
        public async void Put_IfUserDoesNotExist_ReturnsNotFoundResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockUserRepository.Setup(rep => rep.CheckIfExistsAsync(1))
                .ReturnsAsync(false);

            //Act.
            var result = await controller.Put(
                It.IsAny<UserModel>(),
                It.IsAny<int>(),
                It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            mockLogger.VerifyLogger(LogLevel.Debug, "No user found", times);
        }

        [Fact]
        public async void Put_IfUserAiepIdDoesNotEqualUserModelAiepId_UpdatesEducationerAclAndReturnsBadRequestObjectResult()
        {
            //Arrange.
            var userModel = new UserModel { AiepId = 2 };
            var oldUserModel = new UserModel { AiepId = 1 };
            var oldUserEntity = new User { AiepId = 1 };
            mockUserRepository.Setup(rep => rep.FindOneAsync<User>(1, System.Threading.CancellationToken.None))
                .ReturnsAsync(oldUserEntity);
            mockUserRepository.Setup(rep => rep.CheckIfExistsAsync(1))
                .ReturnsAsync(true);
            mockUserRepository.Setup(rep => rep.UpdateUserWithRole(userModel, 3))
                .ReturnsAsync(new RepositoryResponse<UserModel>());
            mockUserRepository.Setup(rep => rep.UpdateEducationerAclAsync(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<bool>
                {
                    ErrorList = new List<string> { "TestError" }
                });

            //Act.
            var result = await controller.Put(
                userModel,
                1,
                It.IsAny<int>()
            );

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);
        }

        [Fact]
        public async void Put_IfUserAiepIdEqualUserModelAiepId_ReturnsOkResult()
        {
            //Arrange.
            var userModel = new UserModel { AiepId = 2 };
            var userEntity = new User { AiepId = 2 };
            mockUserRepository.Setup(rep => rep.FindOneAsync<User>(1, System.Threading.CancellationToken.None))
                .ReturnsAsync(userEntity);
            mockUserRepository.Setup(rep => rep.CheckIfExistsAsync(1))
                .ReturnsAsync(true);
            mockUserRepository.Setup(rep => rep.UpdateUserWithRole(userModel, 3))
                .ReturnsAsync(new RepositoryResponse<UserModel>());

            //Act.
            var result = await controller.Put(
                userModel,
                1,
                3
            );

            //Assert.
            var response = Assert.IsType<OkResult>(result);
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void SetCurrentUserAiepId_IfAiepIdHasValueAndModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            controller.ModelState.AddModelError("Key", "TestError");

            //Act.
            var result = await controller.SetCurrentUserAiepId(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestError", value);

            mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
        }

        [Fact]
        public async void SetCurrentUserAiepId_IfAiepIdHasNoValueAndModelStateIsValid()
        {
            //Arrange.
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var webUser = controller.User;
            mockUserService.Setup(s => s.GetUserId(webUser)).Returns(1);

            mockUserRepository.Setup(rep =>
                    rep.SetUserCurrentAiepId(1, 5))
                .ReturnsAsync(new RepositoryResponse<object> { Content = new object() });

            //Act.
            var result = await controller.SetCurrentUserAiepId(5);

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<object>(response.Value);
        }
    }
}


