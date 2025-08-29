using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.Service.FileStorage;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Repository.ReleaseNoteRepository;
using SpecialEducationPlanning
.Business.Repository.UserReleaseInfoRepository;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    public class ReleaseInfoControllerTest : BaseTest
    {
        private readonly ReleaseInfoController controller;
        private const int foundId = 1;
        private const int notFoundId = 5;

        public ReleaseInfoControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            var repositoryMock = new Mock<IReleaseInfoRepository>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.GetAllAsync<ReleaseInfo>(CancellationToken.None)).ReturnsAsync(new Collection<ReleaseInfo>());
            repositoryMock.Setup(x => x.FindOneAsync<ReleaseInfo>(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync((int id, CancellationToken c) =>
            {
                return id == foundId ? new ReleaseInfo() { DocumentPath = "dummypath" } : null;
            });

            repositoryMock.Setup(x => x.FindOneAsync<ReleaseInfo>(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync((int id, CancellationToken c) =>
            {
                return id == notFoundId ? null : new ReleaseInfo { Id = id, DocumentPath = "dummypath" };
            });

            repositoryMock.Setup(x => x.FindOneAsync<ReleaseInfo>(notFoundId, CancellationToken.None))
                .ReturnsAsync((ReleaseInfo)null);

            repositoryMock.Setup(x => x.ApplyChangesAsync(It.IsAny<ReleaseInfo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReleaseInfo model, CancellationToken token) => model);

            repositoryMock.Setup(x => x.CheckIfExistsAsync(It.IsAny<int>())).ReturnsAsync((int id) => id == foundId ? true : false);
            repositoryMock.Setup(x => x.Remove(It.IsAny<int>())).Verifiable();
            repositoryMock.Setup(x => x.GetNewestReleaseInfoDocumentAsync()).ReturnsAsync(new RepositoryResponse<ReleaseInfoModel>() { Content = null });




            var fileStorageMock = new Mock<IFileStorageService<AzureStorageConfiguration>>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<ReleaseInfoController>>(MockBehavior.Default);
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            var userReleaseInfoRepository = new Mock<IUserReleaseInfoRepository>(MockBehavior.Strict);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(ReleaseInfoProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());


            var mapperMock = new Mock<IObjectMapper>();

            mapperMock.Setup(m => m.Map<ReleaseInfo, ReleaseInfoModel>(It.IsAny<ReleaseInfo>()))
              .Returns((ReleaseInfoModel)null);

            controller = new ReleaseInfoController(repositoryMock.Object, fileStorageMock.Object, userServiceMock.Object, userReleaseInfoRepository.Object, loggerMock.Object, mapper.Object);
        }

        private Stream GetFileStreamMocked(string path)
        {
            var fileStreamMock = new Mock<FileStream>(path, FileMode.Open);
            // Setup mock file using a memory stream
            var content = "Demo txt";
            var fileName = "demo.txt";
            var contentType = "text/plain";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }

        [Fact]
        public async Task GetAllTestAsync()
        {
            var result = await controller.GetAll() as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetNotFoundAsync()
        {
            var result = await controller.Get(notFoundId) as NotFoundResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetFoundAsync()
        {
            var result = await controller.Get(foundId) as OkObjectResult;
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task PostBadModel()
        {
            var model = new ReleaseInfoModel();
            this.controller.ModelState.AddModelError("id", "Invalid Date");
            var result = await controller.Post(model) as BadRequestObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostAsync()
        {
            var model = new ReleaseInfoModel();
            var result = await controller.Post(model) as OkObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task PutBadModel()
        {
            var model = new ReleaseInfoModel
            {
                Id = foundId
            };
            this.controller.ModelState.AddModelError("id", "Invalid Date");
            var result = await controller.Put(model.Id, model) as BadRequestObjectResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PutNotFoundTest()
        {
            var model = new ReleaseInfoModel
            {
                Id = notFoundId
            };
            var response = await controller.Put(notFoundId, model) as NotFoundResult;
            Assert.Equal(404, response.StatusCode);
        }

        [Fact]
        public async Task PutTest()
        {
            //Arrange
            var model = new ReleaseInfoModel
            {
                Id = foundId
            };

            // Act  
            var result = await controller.Put(model.Id, model) as OkObjectResult;

            Assert.Equal((int)result.StatusCode, (int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteNotFoundASync()
        {
            var result = await this.controller.Delete(notFoundId) as NotFoundResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteASync()
        {
            var result = await this.controller.Delete(foundId) as OkResult;
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetReleaseInfoDocumentNotFoundAsync()
        {
            var result = await this.controller.GetReleaseInfoDocument(notFoundId) as NotFoundResult;
            Assert.Equal(result.StatusCode, (int)HttpStatusCode.NotFound);
        }

    }
}
