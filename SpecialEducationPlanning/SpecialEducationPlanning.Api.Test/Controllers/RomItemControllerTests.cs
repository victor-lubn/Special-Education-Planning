using System.Collections.Generic;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Api.Test.Support;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Controllers
{
    [Trait("Controller", "")]
    [Trait("Unit", "")]
    public class RomItemControllerTests : BaseTest
    {
        private readonly RomItemController controller;
        private readonly Mock<IRomItemRepository> mockRomItemRepository;
        private readonly Mock<ILogger<RomItemController>> mockLogger;

        public RomItemControllerTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockRomItemRepository = new Mock<IRomItemRepository>();
            mockLogger = new Mock<ILogger<RomItemController>>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(RomItemModelProfile));
                cfg.AddProfile(typeof(VersionModelProfile));
                cfg.AddProfile(typeof(EndUserModelProfile));


            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            controller = new RomItemController(mockRomItemRepository.Object, mockLogger.Object, mapper.Object);
        }

        [Fact]
        public async void GetAllAsync_ReturnsRepositoryResponseWithRomItemModelsCollection()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            var testRomItemModelsCollection = new List<RomItemModel>
            {
                new RomItemModel {ItemName = "Model1"}
            };

            var testRomItemModelsCollectionEntity = new List<RomItem>
            {
                new RomItem {ItemName = "Model1"}
            };

            mockRomItemRepository.Setup(rep => rep.GetAllAsync<RomItem>(System.Threading.CancellationToken.None))
                .ReturnsAsync(testRomItemModelsCollectionEntity);

            //Act.
            var result = await controller.GetAllAsync();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<RomItemModel>>(response.Value);
            Assert.Contains(value, x => x.ItemName == "Model1");

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called GetAllAsync", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call GetAllAsync -> return All rom items", times);
        }

        [Fact]
        public async void Get_IfRomItemIsNull_ReturnsNotFoundObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await controller.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Get", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "No RomItem found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Get -> return Not found", times);
        }

        [Fact]
        public async void Get_IfRomItemIsNOTNull_ReturnsRepositoryResponseWithRomItemModel()
        {
            //Arrange.
            var testPermissionModel = new RomItemModel { ItemName = "TestRomItemModel" };
            var testPermissionEntity = new RomItem { ItemName = "TestRomItemModel" };
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockRomItemRepository.Setup(rep => rep.FindOneAsync<RomItem>(It.IsAny<int>(), System.Threading.CancellationToken.None))
                .ReturnsAsync(testPermissionEntity);

            //Act.
            var result = await controller.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<RomItemModel>(response.Value);
            Assert.Equal("TestRomItemModel", value.ItemName);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Get", times);
        }

        [Fact]
        public async void Post_IfModelStateIsInvalid_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            controller.ModelState.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await controller.Post(It.IsAny<RomItemModel>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<string>>(response.Value);
            Assert.Contains(value, x => x.Contains("TestError"));

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Post", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Post -> return Bad request", times);
        }

        [Fact]
        public async void Post_IfRomRepositoryContentIsNotNull_ReturnsBadRequestObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockRomItemRepository.Setup(rep => rep.GetRomItemByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResponse<RomItem> { Content = new RomItem() });

            //Act.
            var result = await controller.Post(new RomItemModel());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal("EntityAlreadyExist", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Post", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "EntityAlreadyExist", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Post -> return Bad request Entity exists", times);
        }

        [Fact]
        public async void Post_IfRomRepositoryContentIsNull_ReturnsJsonResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockRomItemRepository.Setup(rep => rep.GetRomItemByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResponse<RomItem> { Content = null });

            //Act.
            var result = await controller.Post(new RomItemModel());

            //Assert.
            Assert.IsType<JsonResult>(result);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Post", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Post -> return Created rom item", times);
        }

        [Fact]
        public async void Put_IfModelStateIsInvalid_ReturnsBadRequest()
        {
            //Arrange.
            controller.ModelState.AddModelError("Key", "TestError");
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await controller.Put(It.IsAny<int>(), It.IsAny<RomItemModel>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<string>>(response.Value);
            Assert.Contains(value, x => x.Contains("TestError"));

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Put", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "ModelState is not valid TestError", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Put -> return Bad request", times);
        }

        [Fact]
        public async void Put_IfRomItemNotExists_ReturnsBadRequest()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockRomItemRepository.Setup(rep => rep.CheckIfExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            //Act.
            var result = await controller.Put(It.IsAny<int>(), It.IsAny<RomItemModel>());

            //Assert.
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, response.StatusCode);
            var value = Assert.IsType<string>(response.Value);
            Assert.Equal("EntityNotFound", value);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Put", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "EntityNotFound", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Put -> return Bad request Not found", times);
        }

        [Fact]
        public async void Put_IfRomItemExists_ReturnsJsonResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockRomItemRepository.Setup(rep => rep.CheckIfExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            mockRomItemRepository.Setup(rr => rr.FindOneAsync<RomItem>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RomItem());

            //Act.
            var result = await controller.Put(It.IsAny<int>(), It.IsAny<RomItemModel>());

            //Assert.
            Assert.IsType<JsonResult>(result);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Put", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Put -> return Updated rom item", times);
        }

        [Fact]
        public async void Delete_IfRomItemIsNotExist_ReturnsNotFoundResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await controller.Delete(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Delete", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItem not found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Delete -> return Not found", times);
        }

        [Fact]
        public async void Delete_IfRomItemIsExist_ReturnsOkResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockRomItemRepository.Setup(rep => rep.FindOneAsync<RomItem>(It.IsAny<int>(), System.Threading.CancellationToken.None))
                .ReturnsAsync(new RomItem());

            //Act.
            var result = await controller.Delete(It.IsAny<int>());

            //Assert.
            Assert.IsType<OkResult>(result);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController called Delete", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "RomItemController end call Delete -> return Ok", times);
        }
    }
}
