using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;
using SpecialEducationPlanning
.Api.Test.Support;

using Koa.Domain;

using Moq;
using SpecialEducationPlanning
.Api.Service.FittersPack;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class VersionControllerTest : BaseTest
    {
        private readonly VersionController controller;
        private readonly Mock<IVersionRepository> mockVersionRepository;
        private readonly Mock<IPlanRepository> mockPlanRepository;
        private readonly Mock<ILogger<VersionController>> mockLogger;

        public VersionControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

            mockLogger = new Mock<ILogger<VersionController>>();
            mockPlanRepository = new Mock<IPlanRepository>();
            mockVersionRepository = new Mock<IVersionRepository>(MockBehavior.Strict);
            mockVersionRepository.Setup(repo => repo.GetAllAsync<Domain.Entity.Version>(CancellationToken.None)).ReturnsAsync(GetAllEntity());
            mockVersionRepository.Setup(m => m.CheckIfExistsAsync(It.IsAny<int>())).ReturnsAsync((int x) => {
                if (x == 1)
                {
                    return true;
                }
                return false;
            });
            mockVersionRepository.Setup(repo => repo.FindOneAsync<Domain.Entity.Version>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int x, CancellationToken c) =>
            {
                if (x == 1)
                {
                    return GetSingleEducationEntity();
                }
                return null;
            });
            mockVersionRepository.Setup(m => m.Remove(It.IsAny<int>())).Verifiable();
            mockVersionRepository.Setup(repo => repo.ApplyChangesAsync(It.IsAny<Domain.Entity.Version>(), CancellationToken.None))
                .ReturnsAsync((Domain.Entity.Version v, CancellationToken c) => v);

            mockVersionRepository.Setup(repo => repo.UpdateVersionAsync(It.IsAny<Version>())).ReturnsAsync(new RepositoryResponse<VersionModel>(new VersionModel() { Id = 1 }));
            mockVersionRepository.Setup(repo => repo.GetRomAndPreviewInfo(It.IsAny<int>())).ReturnsAsync((int x) =>
            {
                if (x == 1)
                {
                    return GetVersionRepositoryResponse().GetAwaiter().GetResult();
                }
                return null;
            });

            var mockActionRepo = new Mock<IActionRepository>(MockBehavior.Strict);
            mockActionRepo.Setup(repo =>
                repo.CreateAction<BaseModel>(It.IsAny<ActionType>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(default(object)));

            mockPlanRepository.Setup(repo => repo.FindOneAsync<Plan>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int i, CancellationToken c) =>
            {
                if (i > 0) { return GetSinglePlanEntity(); }
                return null;
            });
            mockPlanRepository.Setup(repo => repo.ApplyChangesAsync(It.IsAny<Plan>(), CancellationToken.None))
                .ReturnsAsync((Plan p, CancellationToken c) => p);

            var mockCommentRepo = new Mock<ICommentRepository>();

            var mockFileStorageService = new Mock<IFileStorageService<AzureStorageConfiguration>>();
            mockFileStorageService.Setup(m => m.DownloadAsync(It.IsAny<string>())).ReturnsAsync(GetDownloadStream());

            var mockFittersPackStorageService = new Mock<IFileStorageService<AzureStorageFittersPackConfiguration>>();
            mockFittersPackStorageService.Setup(m => m.DownloadAsync(It.IsAny<string>())).ReturnsAsync(GetDownloadStream());

            var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            mockUserService.Setup(m => m.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(1);
            mockUserService.Setup(m => m.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(1);
            mockUserService.Setup(x => x.GetUserIdentifier(It.IsAny<ClaimsIdentity>())).Returns("User");

            var mockFittersPackService = new Mock<IFittersPackService>(MockBehavior.Strict);

            var mockUserRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockUserRepo.Setup(u => u.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new RepositoryResponse<UserModel>() { Content = new UserModel() });

            var mockRomRepo = new Mock<IRomItemRepository>();
            var mockCatalogRepo = new Mock<ICatalogRepository>();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(VersionModelProfile));
                cfg.AddProfile(typeof(PlanModelProfile));
                cfg.AddProfile(typeof(RomItemModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            controller = new VersionController(
                mockVersionRepository.Object,
                mockActionRepo.Object,
                mockPlanRepository.Object,
                mockRomRepo.Object,
                mockCatalogRepo.Object,
                mockCommentRepo.Object,
                mockFileStorageService.Object,
                mockFittersPackStorageService.Object,
                mockLogger.Object,
                mockUserRepo.Object,
                mockUserService.Object,
                mapper.Object,
                mockFittersPackService.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } }
            };
        }

        private Stream GetDownloadStream()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Downloaded file"));
            return stream;
        }

        private async Task<RepositoryResponse<Domain.Entity.Version>> GetVersionRepositoryResponse()
        {
            return new RepositoryResponse<Domain.Entity.Version>(GetSingleVersion());
        }

        private Domain.Entity.Version GetSingleVersion()
        {
            return new Domain.Entity.Version()
            {
                Id = 1,
                Rom = "Rom",
                RomPath = "RomPath",
                Preview = "Preview",
                PreviewPath = "PreviewPath"
            };
        }

        private VersionModel GetSingleEducation()
        {
            var model = new VersionModel
            {
                Id = 1
            };
            return model;
        }

        private Domain.Entity.Version GetSingleEducationEntity()
        {
            var model = new Domain.Entity.Version
            {
                Id = 1
            };
            return model;
        }


        private PlanModel GetSinglePlan()
        {
            var model = new PlanModel
            {
                Id = 1
            };
            return model;
        }

        private Plan GetSinglePlanEntity()
        {
            var model = new Plan
            {
                Id = 1
            };
            return model;
        }


        private List<VersionModel> GetAll()
        {
            var Educationers = new List<VersionModel>
            {
                new VersionModel
                {
                    Id = 1
                }
            };
            return Educationers;
        }

        private List<Domain.Entity.Version> GetAllEntity()
        {
            var Educationers = new List<Domain.Entity.Version>
            {
                new Domain.Entity.Version()
                {
                    Id = 1
                }
            };
            return Educationers;
        }

        [Fact]
        public async Task DeleteNotFoundTest()
        {
            var response = await controller.Delete(2) as NotFoundResult;
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTest()
        {
            var response = await controller.Delete(1) as OkResult;
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task GetAllTest()
        {
            //Act
            var result = await controller.GetAll() as OkObjectResult;
            var model = result.Value as List<VersionModel>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(GetAll().Count, model.Count);
        }

        [Fact]
        public async Task GetByIdNotFoundTesT()
        {
            var response = await controller.Get(2) as NotFoundResult;
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task GetByIdTest()
        {
            //Arrange
            var id = 1;

            // Act
            var response = await controller.Get(id) as OkObjectResult;
            var model = response.Value as VersionModel;

            // Assert
            Assert.Equal(id, model.Id);
        }

        public async Task PostTest()
        {
            var model = new VersionModel();
            var response = await controller.Post(model) as CreatedAtActionResult;
            Assert.Equal(201, response.StatusCode);
        }

        [Fact]
        public async Task PostWithModelErrorsTest()
        {
            //Arrange
            controller.ModelState.AddModelError("value", "The model has an error");

            var response = await controller.Post(null) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutNotFoundTest()
        {
            var newmodel = new VersionModel
            {
                Id = 2
            };
            var response = await controller.Put(2, newmodel) as NotFoundResult;
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task PutTest()
        {
            //Arrange
            var model = new VersionModel { Id = 1, PlanId = 1 };

            // Act  
            var result = await controller.Put(1, model) as OkObjectResult;

            Assert.Equal((int)result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task Put_IfPlanNotFound_ReturnsNotFoundResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockVersionRepository.Setup(rep => rep.FindOneAsync<Domain.Entity.Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Domain.Entity.Version());

            mockPlanRepository.Setup(rep => rep.FindOneAsync<Plan>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync((Plan)null);

            var model = new VersionModel();

            //Act.
            var result = await controller.Put(It.IsAny<int>(), model);

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "Plan not found", times);
        }

        [Fact]
        public async Task Put_IfUserIdentityIsNull_ReturnsNotFoundResult()
        {
            //Arrange.
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal { Identities = { } };

            mockVersionRepository.Setup(rep => rep.FindOneAsync<Domain.Entity.Version>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Domain.Entity.Version());

            mockPlanRepository.Setup(rep => rep.FindOneAsync<Plan>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Plan());

            //Act.
            var result = await controller.Put(It.IsAny<int>(), new VersionModel());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task Put_IfModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var model = new VersionModel();
            controller.ModelState.AddModelError("Key", "TestErrorMessage");

            //Act.
            var result = await controller.Put(It.IsAny<int>(), model);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<List<string>>(response.Value);
            Assert.Contains("TestErrorMessage", value);
        }

        [Fact]
        public void GetPreviewTest()
        {
            var response = controller.GetPreview(1);

            var value = ((FileStreamResult)response.Result).FileDownloadName;
            Assert.Equal("Preview", value);
        }

        [Fact]
        public void GetPreviewNotFoundTest()
        {
            var response = controller.GetPreview(2);
            var value = response.Result as NotFoundResult;
            Assert.Equal(404, value.StatusCode);
        }

        [Fact]
        public void GetPlanRomTest()
        {
            var response = controller.GetRom(1);
            var value = ((FileStreamResult)response.Result).FileDownloadName;
            Assert.Equal("Rom", value);
        }

        [Fact]
        public void GetPlanRomNotFoundTest()
        {
            var response = controller.GetRom(2);
            var value = response.Result as NotFoundResult;
            Assert.Equal(404, value.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsOkResult()
        {
            //Arrange.
            var model = new VersionModel { RomItems = new List<RomItemModel> { new RomItemModel() }, PlanId = 1 };

            mockVersionRepository.Setup(rep => rep.CreateVersionAsync(It.IsAny<VersionModel>()))
                .ReturnsAsync(new VersionModel { Id = 1, UpdatedDate = DateTime.MinValue });

            mockPlanRepository.Setup(rep => rep.GetWithNavigationsAsync<Plan>(
                    model.PlanId,
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new Plan { MasterVersionId = 2, UpdatedDate = DateTime.MaxValue });

            //Act.
            var result = await controller.Post(model);

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<VersionModel>(response.Value);
            Assert.Equal(1, value.Id);
        }

        [Fact]
        public async Task Post_IfUserIdentityIsNull_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            var model = new VersionModel { RomItems = new List<RomItemModel> { new RomItemModel() }, PlanId = 1 };

            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal { Identities = { } };

            mockVersionRepository.Setup(rep => rep.CreateVersionAsync(It.IsAny<VersionModel>()))
                .ReturnsAsync(new VersionModel { Id = 1, UpdatedDate = DateTime.MinValue });

            mockPlanRepository.Setup(rep => rep.GetWithNavigationsAsync<Plan>(model.PlanId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new Plan { MasterVersionId = 2, UpdatedDate = DateTime.MaxValue });

            //Act.
            var result = await controller.Post(model);

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal("UndefinedUser", value);
        }
    }
}

