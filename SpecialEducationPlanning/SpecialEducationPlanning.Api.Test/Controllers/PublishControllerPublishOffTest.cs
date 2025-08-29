using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SpecialEducationPlanning
.Api.Configuration.Strategy;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Model.PublishServiceModel;
using SpecialEducationPlanning
.Api.Service.FeatureManagement;
using SpecialEducationPlanning
.Api.Service.Publish;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class PublishControllerPublishOffTest : BaseTest
    {
        private readonly PublishController controller;
        const int existingPlanId = 2;
        const int existingVersionId = 7;
        const int nonExistingId = 100;

        public PublishControllerPublishOffTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var mockLogger = new Mock<ILogger<PublishController>>();

            var mockPublishService = new Mock<IPublishService>(MockBehavior.Strict);
            mockPublishService.Setup(m => m.PublishPlanFileAsync(It.IsAny<PublishVersionModel>(), It.IsAny<Stream>())).ReturnsAsync(new RepositoryResponse<string>());
            mockPublishService.Setup(m => m.PublishVersionAsync(It.IsAny<PublishVersionModel>())).ReturnsAsync(new RepositoryResponse<string>());
            mockPublishService.Setup(service => service.CreateLegacyZipPlanFromStream(It.IsAny<Stream>())).ReturnsAsync(new MemoryStream());

            var mockFeatureManagementServiceOff = new Mock<IFeatureManagementService>(MockBehavior.Strict);
            mockFeatureManagementServiceOff.Setup(a => a.GetFeatureStringFlagAsync(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(FeatureFlagEnum.Off);

            var planMock = new Mock<IPlanRepository>(MockBehavior.Strict);
            planMock.Setup(repo => repo.FindOneAsync<Plan>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int id, CancellationToken c) =>
            {
                return (id == existingPlanId) ? new Plan() { MasterVersionId = existingVersionId } : null;
            });
            planMock.Setup(repo => repo.GetPlanWithHousingTypeHousingSpecs(It.IsAny<int>()))
                .ReturnsAsync(new Plan());

            var versionMock = new Mock<IVersionRepository>(MockBehavior.Strict);
            versionMock.Setup(repo => repo.FindOneAsync<Domain.Entity.Version>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int id, CancellationToken c) =>
            {
                return (id == existingVersionId) ? new Domain.Entity.Version() : null;
            });
            versionMock.Setup(repo => repo.GetRomPathById(It.IsAny<int>()))
                .ReturnsAsync((int id) => (id == existingVersionId) ? new RepositoryResponse<string>() { Content = "plans\\my.rom" } : new RepositoryResponse<string>());
            
            versionMock.Setup(repo => repo.GetVersionWithPlanProjectAiep(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Version());

            var actionRepoMock = new Mock<IActionRepository>(MockBehavior.Strict);
            actionRepoMock.Setup(repo => repo.CreateAction<PlanModel>(Domain.Enum.ActionType.PlanPublished, string.Empty, It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(default(object)));

            var builderRepoMock = new Mock<IBuilderRepository>(MockBehavior.Strict);
            builderRepoMock.Setup(b => b.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int x, CancellationToken c) =>
            {
                return new Builder();
            });

            var fileStorageServiceMock = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Strict);
            fileStorageServiceMock.Setup(service => service.DownloadAsync(It.IsAny<string>())).ReturnsAsync(new MemoryStream());

            var mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            mockUserService.Setup(m => m.GetUserId(It.IsAny<ClaimsIdentity>())).Returns(1);
            mockUserService.Setup(m => m.GetUserAiepId(It.IsAny<ClaimsIdentity>())).Returns(1);

            var mockUserRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockUserRepo.Setup(u => u.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new RepositoryResponse<UserModel>() { Content = new UserModel() });
            mockUserService.Setup(x => x.GetUserIdentifier(It.IsAny<ClaimsIdentity>())).Returns("User");

            var mockCountryConfiguration = new Mock<IOptions<CountryConfiguration>>(MockBehavior.Default);
            CountryConfiguration app = new CountryConfiguration() { StrategyIdentifier = "IRL" };
            mockCountryConfiguration.Setup(x => x.Value).Returns(app);
            

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));



            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(PlanModelProfile));
                cfg.AddProfile(typeof(VersionModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            var mockPublishProjectService = new Mock<IPublishProjectService>(MockBehavior.Strict);
            mockPublishProjectService.Setup(m => m.SendRomItemsToCreatioAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<string>());

            var mockProjectRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockProjectRepo.Setup(m => m.GetProjectPlans(It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PlanModel>>() { Content = new List<PlanModel>()});

            var mockPublish3DcService = new Mock<IPublish3DcService>(MockBehavior.Strict);
            mockPublish3DcService.Setup(m => m.PublishVersionAsync(It.IsAny<PublishVersionModel>())).ReturnsAsync(new RepositoryResponse<string>());

            controller = new PublishController(mockPublishService.Object,
                planMock.Object,
                versionMock.Object,
                actionRepoMock.Object,
                this.LoggerFactory.CreateLogger<PublishController>(),
                mockUserService.Object,
                mockFeatureManagementServiceOff.Object,
                mockCountryConfiguration.Object,
                mapper.Object,
                mockPublishProjectService.Object,
                mockProjectRepo.Object,
                mockPublish3DcService.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = user } }
            };


        }

        /// <summary>
        /// Publish Plan by Master Version ID, passing a Plan ID. This will search the MasterVersion ID and send 
        /// this Version ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishPlanTestAsync()
        {
            var publishPlanPostModel = new PublishPlanPostModel
            {
                PlanId = existingPlanId
            };
            var response = await this.controller.PublishPlan(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Publish Version by existing Version ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishVersionTestAsync()
        {
            var publishPlanPostModel = new PublishVersionModel
            {
                VersionId = existingVersionId,
                Comments = ""

            };
            var response = await this.controller.PublishVersion(publishPlanPostModel) as OkResult;
            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        }

    }
}
