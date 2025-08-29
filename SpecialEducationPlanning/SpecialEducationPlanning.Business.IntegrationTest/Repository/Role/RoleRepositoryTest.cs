using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;

using Moq;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.Role
{
    public class RoleRepositoryTest : BaseTest
    {
        private readonly IRoleRepository repository;
        private readonly IPermissionRepository permissionRepo;
        private readonly IBuilderRepository builderRepo;
        private readonly IUserRepository userRepo;
        private readonly Mock<IObjectMapper> mockObjectMapper;


        public RoleRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            repository = fixture.ServiceProvider.GetRequiredService<IRoleRepository>();
            permissionRepo = fixture.ServiceProvider.GetRequiredService<IPermissionRepository>();
            builderRepo = fixture.ServiceProvider.GetRequiredService<IBuilderRepository>();
            userRepo = fixture.ServiceProvider.GetRequiredService<IUserRepository>();
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);

        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
            Assert.NotNull(permissionRepo);
        }

        [Fact]
        public async Task GetPermissionsAsyncTest()
        {
            var insertPermission = this.permissionRepo.Add(new Permission() { Name = "Test" });
            var roleEntity = await this.repository.GetPermissionsFromRole(insertPermission.Id);
            Assert.NotNull(roleEntity);
        }

        [Fact]
        public async Task GetPermissionsNullAsyncTest()
        {
            var roleEntity = await this.repository.GetPermissionsFromRole(0);
            Assert.Null(roleEntity.Content);
        }

        [Fact]
        public async Task SetPermissionsAsyncTest()
        {
            var insertPermission = this.permissionRepo.Add(new Permission() { Name = "Test" });
            var roleInsert = this.repository.Add(new Domain.Entity.Role { Name = "Test Role" }); // add

            var setPermission = await this.repository.UpdatePermissions(new RoleModel { Name = "Test Role", Id = 1 }, new List<int>() { 1 });
            Assert.Empty(setPermission.ErrorList);
        }

        [Fact]
        public async Task SetPermissionsWithoutRoleAsyncTest()
        {
            var setPermission = await this.repository.UpdatePermissions(new RoleModel(), new List<int>() { 1 });
            Assert.NotEmpty(setPermission.ErrorList);
        }

        [Fact]
        public async Task GetUserRolesAsyncTest()
        {
            var foundUser = this.AddTestUser();

            var userRoles = await this.repository.GetUserRolesAsync(foundUser.Id);
            Assert.NotNull(userRoles);
            Assert.Empty(userRoles.ErrorList);
        }

        [Fact]
        public async Task GetUserRolesNullAsyncTest()
        {
            var userRoles = await this.repository.GetUserRolesAsync(0);
            Assert.NotEmpty(userRoles.ErrorList);
        }

        [Fact]
        public async Task GetUserPermissionsAsyncTest()
        {
            var foundUser = this.AddTestUser();

            var userPermissions = await this.repository.GetUserPermissionsAsync(foundUser.Id);
            Assert.NotNull(userPermissions);
            Assert.Empty(userPermissions.ErrorList);
        }

        [Fact]
        public async Task GetUserPermissionsWithoutUserAsyncTest()
        {
            var userPermissions = await this.repository.GetUserPermissionsAsync(0);
            Assert.NotEmpty(userPermissions.ErrorList);
        }

        
        [Fact(Skip = "Ignore")]
        public async Task SetUserRolesAsyncTest()
        {
            var foundUser = this.AddTestUser();
            
            var setPermission = await this.repository.SetUserRolesAsync(foundUser.Id, new List<int>() { 1 });
            Assert.NotNull(setPermission);
            Assert.Empty(setPermission.ErrorList);
        }

        [Fact]
        public async Task SetUserRolesWithoutUserAsyncTest()
        {
            var setPermission = await this.repository.SetUserRolesAsync(0, new List<int>() { 1 });
            Assert.NotNull(setPermission);
            Assert.NotEmpty(setPermission.ErrorList);
        }

        private User AddTestUser()
        {
            var user = this.userRepo.Add(new User()
            {
                FirstName = "Test",
                Surname = "Test"
            });
            
            var users = this.userRepo.GetAll<User>();
            var foundUser = users.Where(x => x.FirstName == "Test" && x.Surname == "Test").FirstOrDefault();
            return foundUser;
        }
    }
}