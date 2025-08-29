using System.Collections.Generic;
using System.Linq;

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
    public class PermissionControllerTests : BaseTest
    {
        private readonly Mock<ILogger<PermissionController>> mockLogger;
        private readonly Mock<IPermissionRepository> mockPermissionRepository;
        private readonly PermissionController permissionController;

        public PermissionControllerTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper): base(fixture, outputHelper)
        {
            mockLogger = new Mock<ILogger<PermissionController>>();
            var mockRoleRepository = new Mock<IRoleRepository>();
            mockPermissionRepository = new Mock<IPermissionRepository>();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(PermissionModelProfile));
            });

            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());
            permissionController = new PermissionController(
                mockPermissionRepository.Object,
                mockRoleRepository.Object,
                mockLogger.Object,
                mapper.Object
                );
        }

        [Fact]
        public async void Get_IfRepositoryContentIsNull_ReturnsNotFoundObjectResult()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            //Act.
            var result = await permissionController.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, response.StatusCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController called Get", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "No permission found", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController end call Get -> return Not found", times);
        }

        [Fact]
        public async void Get_IfRepositoryContentNOTNull_ReturnsRepositoryResponseWithPermissionModel()
        {
            //Arrange.
            var testPermissionModel = new PermissionModel { DescriptionCode = "TestPermissionModel" };
            var testPermissionEntity = new Permission { DescriptionCode = "TestPermissionModel" };
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockPermissionRepository.Setup(rep => rep.FindOneAsync<Permission>(It.IsAny<int>(), System.Threading.CancellationToken.None))
                .ReturnsAsync(testPermissionEntity);

            //Act.
            var result = await permissionController.Get(It.IsAny<int>());

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsType<PermissionModel>(response.Value);
            Assert.Equal("TestPermissionModel", value.DescriptionCode);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController called Get", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController end call Get -> return Permission", times);
        }

        [Fact]
        public async void GetAll_ReturnsRepositoryResponseWithPermissionModelsCollection()
        {
            //Arrange.
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            var testPermissionModelCollection = new List<PermissionModel>
            {
                new PermissionModel {DescriptionCode = "Test"},
                new PermissionModel {DescriptionCode = "Test"},
                new PermissionModel {DescriptionCode = "Test"}
            };

            var testPermissionModelCollectionEntity = new List<Permission>
            {
                new Permission {DescriptionCode = "Test"},
                new Permission {DescriptionCode = "Test"},
                new Permission {DescriptionCode = "Test"}
            };

            mockPermissionRepository.Setup(rep => rep.GetAllAsync<Permission>(System.Threading.CancellationToken.None))
                .ReturnsAsync(testPermissionModelCollectionEntity);

            //Act.
            var result = await permissionController.GetAll();

            //Assert.
            var response = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, response.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<PermissionModel>>(response.Value);
            Assert.True(value.All(v => v.DescriptionCode == "Test"));

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController called GetAll", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PermissionController end call GetAll -> return All permissions", times);
        }
    }
}
