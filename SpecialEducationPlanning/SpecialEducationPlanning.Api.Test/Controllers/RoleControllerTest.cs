using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

using Moq;

using SpecialEducationPlanning
.Api.Service.User;

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
    public class RoleControllerTest : BaseTest
    {
        private readonly int existingRoleId = 1;
        private readonly int nonExistingRoleId = 99;

        private readonly Mock<IRoleRepository> _mockRoleRepository;

        private readonly RoleController roleController;

        private readonly Mock<IUserService> _mockUserService;

        public RoleControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            _mockRoleRepository = new Mock<IRoleRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<IUserService>(MockBehavior.Default);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(RoleModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

            roleController = new RoleController(
                _mockRoleRepository.Object,
                this.LoggerFactory.CreateLogger<RoleController>(),
                _mockUserService.Object,
                mapper.Object
            );
        }

        #region Private Methods
        private RoleModel RoleExistingInstance()
        {
            return new RoleModel
            {
                Id = existingRoleId
            };
        }

        private ICollection<RoleModel> RoleListInstance()
        {
            ICollection<RoleModel> roles = new List<RoleModel>
            {
                RoleExistingInstance()
            };
            return roles;
        }
        private Role RoleExistingInstanceEntity()
        {
            return new Role
            {
                Id = existingRoleId
            };
        }

        private ICollection<Role> RoleListInstanceEntity()
        {
            ICollection<Role> roles = new List<Role>
            {
                RoleExistingInstanceEntity()
            };
            return roles;
        }


        private RolePermissionModel RolePermissionExistingInstance()
        {
            return new RolePermissionModel
            {
                Id = existingRoleId
            };
        }

        private PermissionModel PermissionExistingInstance()
        {
            return new PermissionModel
            {
                Id = existingRoleId
            };
        }

        private ICollection<PermissionModel> PermissionListInstance()
        {
            ICollection<PermissionModel> roles = new List<PermissionModel>
            {
                PermissionExistingInstance()
            };
            return roles;
        }

        private PermissionAssignedAvailableModel PermissionAssignedAvailableInstance()
        {
            return new PermissionAssignedAvailableModel()
            {
                PermissionAssigned = PermissionListInstance(),
                PermissionsAvailable = PermissionListInstance()
            };
        }
        #endregion

        #region Test Methods

        #region Delete
        [Fact]
        public async void Delete_NonExistingRegion_NotFound()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(nonExistingRoleId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await roleController.Delete(nonExistingRoleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Delete_ExistingArea_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(existingRoleId, CancellationToken.None))
                .ReturnsAsync(RoleExistingInstanceEntity());

            _mockRoleRepository.Setup(rr => rr.Remove(existingRoleId))
                .Verifiable();

            // Act 
            var result = await roleController.Delete(existingRoleId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #region Get

        #region Get by ID
        [Fact]
        public async void Get_NonExistingRole_NotFound()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(nonExistingRoleId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await roleController.Get(nonExistingRoleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Get_ExistingRole_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(existingRoleId, CancellationToken.None))
                .ReturnsAsync(RoleExistingInstanceEntity());

            // Act 
            var result = await roleController.Get(existingRoleId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #region Get All
        [Fact]
        public async void GetAll_Ok()
        {
            // Arrange 
            _mockRoleRepository.Setup(rr => rr.GetAllAsync<Role>(CancellationToken.None))
                .ReturnsAsync(RoleListInstanceEntity());

            // Act
            var result = await roleController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<RoleModel>>((result as OkObjectResult).Value);
        }
        #endregion

        // To do
        #region Get Roles Filtered

        #endregion

        #region Get Permissions From Role
        [Fact]
        public async void GetPermissionsFromRole_Ok()
        {
            // Arrange 
            _mockRoleRepository.Setup(rr => rr.GetPermissionsFromRole(existingRoleId))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<PermissionModel>>(PermissionListInstance()));

            // Act
            var result = await roleController.GetPermissionsFromRole(existingRoleId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<PermissionModel>>((result as OkObjectResult).Value);
        }
        #endregion

        #region Get Permissions Assigned and Available
        [Fact]
        public async void GetPermissionsAssignedAvailable_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.GetPermissionsAssignedAvailable(existingRoleId))
               .ReturnsAsync(new RepositoryResponse<PermissionAssignedAvailableModel>(PermissionAssignedAvailableInstance()));

            // Act
            var result = await roleController.GetPermissionsAssignedAvailable(existingRoleId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        // To do
        #region Get Role User Permissions

        #endregion

        #endregion

        #region Post
        [Fact]
        public async void Post_ValidModel_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.ApplyChangesAsync(It.IsAny<Role>(), CancellationToken.None))
                .ReturnsAsync(RoleExistingInstanceEntity());

            // Act
            var result = await roleController.Post(RoleExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<RoleModel>((result as OkObjectResult).Value);
        }

        [Fact]
        public async void Post_ModelError_BadRequest()
        {
            // Arrange
            roleController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await roleController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #region Role Permission
        [Fact]
        public async void SetRolePermissions_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.SetPermissions(It.IsAny<RolePermissionModel>()))
                .ReturnsAsync(new RepositoryResponse<RoleModel>(RoleExistingInstance()));

            // Act
            var result = await roleController.SetRolePermissions(RolePermissionExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        #endregion

        #endregion

        #region Put   
        [Fact]
        public async void Put_ModelError_BadRequest()
        {
            // Arrange
            roleController.ModelState.AddModelError("id", "id is null");

            // Act
            var result = await roleController.Put(RoleExistingInstance().Id, RoleExistingInstance());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Put_NonExistingRegion_NotFound()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(nonExistingRoleId, CancellationToken.None))
                .ReturnsAsync(() => null);

            // Act
            var result = await roleController.Put(nonExistingRoleId, It.IsAny<RoleModel>());

            // Assert            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Put_ExistingRole_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.FindOneAsync<Role>(existingRoleId, CancellationToken.None))
                .ReturnsAsync(RoleExistingInstanceEntity());

            _mockRoleRepository.Setup(rr => rr.ApplyChangesAsync(It.IsAny<Role>(), CancellationToken.None))
                .ReturnsAsync(RoleExistingInstanceEntity());

            // Act
            var result = await roleController.Put(existingRoleId, RoleExistingInstance());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        #region Update Role Permissions
        [Fact]
        public async void UpdateRolePermissions_Ok()
        {
            // Arrange
            _mockRoleRepository.Setup(rr => rr.UpdatePermissions(It.IsAny<RoleModel>(), It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new RepositoryResponseGeneric());

            // Act
            var result = await roleController.UpdateRolePermissions(existingRoleId, RolePermissionExistingInstance());

            // Assert
            Assert.IsType<OkResult>(result);
        }
        #endregion

        #endregion

        #endregion
    }
}