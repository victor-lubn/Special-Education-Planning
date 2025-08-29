using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Domain.Specification.Search;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using Koa.Domain.Search.Page;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class RoleRepositoryTest : BaseTest
    {
        private readonly int existingId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly RoleRepository repository;

        public RoleRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new RoleRepository(
                this.LoggerFactory.CreateLogger<RoleRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }
        #region Func : GetPermissionsFromRole
        [Fact]
        public void GetPermissionsFromRole_GetPermissionsFromRoleRoleEntityIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetPermissionsFromRole(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositoryGetPermissions");
        }

        [Fact]
        public void GetPermissionsFromRole_GetPermissionsFromRoleRoleEntityIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Permission, PermissionModel>(It.IsAny<IEnumerable<Permission>>()))
                .Returns(new List<PermissionModel>());

            // Act
            var result = repository.GetPermissionsFromRole(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<IEnumerable<PermissionModel>>>(result.Result);
        }
        #endregion

        #region Func : GetPermissionsAssignedAvailable
        [Fact]
        public void GetPermissionsAssignedAvailable_GetPermissionsAssignedAvailableRoleEntityIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetPermissionsAssignedAvailable(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositoryGetPermissionsAssignedAvailable");
        }

        [Fact]
        public void GetPermissionsAssignedAvailable_GetPermissionsAssignedAvailableRoleEntityIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.GetAll<Permission>())
                .Returns(new List<Permission>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Permission>, IEnumerable<PermissionModel>>(It.IsAny<IEnumerable<Permission>>()))
                .Returns(new LinkedList<PermissionModel>());

            // Act
            var result = repository.GetPermissionsAssignedAvailable(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<PermissionAssignedAvailableModel>>(result.Result);
        }
        #endregion

        #region Func : GetUserPermissionsAsync
        [Fact]
        public void GetUserPermissionsAsync_GetUserPermissionsAsyncRoleEntityIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetUserPermissionsAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void GetUserPermissionsAsync_GetUserPermissionsAsyncRoleEntityIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Permission, PermissionModel>(It.IsAny<IEnumerable<Permission>>()))
                .Returns(new List<PermissionModel>());

            // Act
            var result = repository.GetUserPermissionsAsync(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<IEnumerable<PermissionModel>>>(result.Result);
        }
        #endregion

        #region Func : GetUserRolesAsync
        [Fact]
        public void GetUserRolesAsync_GetUserRolesAsyncEntityIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetUserRolesAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositoryGetUserRolesAsync");
        }

        [Fact]
        public void GetUserRolesAsync_GetUserRolesAsyncEntityIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Role, RoleModel>(It.IsAny<IEnumerable<Role>>()))
                .Returns(new List<RoleModel>());

            // Act
            var result = repository.GetUserRolesAsync(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<IEnumerable<RoleModel>>>(result.Result);
        }
        #endregion

        #region Func : RefreshPermissionListAsync
        [Fact]
        public void RefreshPermissionListAsyncTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<Permission>())
                .Returns(new List<Permission>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Permission>()))
                .Returns(new Permission());

            mockEntityRepositoryKey.Setup(er => er.Remove(It.IsAny<Permission>()))
                .Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.RefreshPermissionListAsync();

            // Assert

            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : SetPermissions
        [Fact]
        public void SetPermissions_SetPermissionsInExistingRoleTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.SetPermissions(new RolePermissionModel());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist Role already exists from SetPermissions");
        }

        [Fact]
        public void SetPermissions_SetPermissionsInNonExistingRoleTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<PermissionRole>()))
                .Returns(new PermissionRole());

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Role>()))
                .Returns(new Role());

            mockObjectMapper.Setup(m => m.Map<RoleModel, Role>(It.IsAny<RoleModel>()))
                .Returns(new Role());

            mockObjectMapper.Setup(m => m.Map<Role, RoleModel>(It.IsAny<Role>()))
                .Returns(new RoleModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

            mockUnitOfWork.Setup(un => un.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Role>()))
                 .Returns(new Role());
            var result = repository.SetPermissions(new RolePermissionModel());

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<RoleModel>>(result.Result);
        }
        #endregion

        #region Func : UpdatePermissions
        [Fact]
        public void UpdatePermissions_UpdatePermissionsExistingRoleTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdatePermissions(new RoleModel(), new List<int>());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositoryUpdatePermissions");
        }

        [Fact]
        public void UpdatePermissions_UpdatePermissionsNullRoleWithPermissionsTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object)
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdatePermissions(new RoleModel() { Id = 0 }, new List<int>());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositorySetPermissions");
        }

        [Fact]
        public void UpdatePermissions_UpdatePermissionsDifferentQueriedPermissionsAndIdsTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object)
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Permission>>()))
                .Returns(new List<Permission>() { new Permission() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdatePermissions(new RoleModel() { Id = 1 }, new List<int>());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepository SetRolePermissionsPermissionsId");
        }

        [Fact]
        public void UpdatePermissions_UpdatePermissionsEqualQueriedPermissionsAndIdsTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object)
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Permission>>()))
                .Returns(new List<Permission>() { new Permission() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Remove(It.IsAny<PermissionRole>()))
                .Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdatePermissions(new RoleModel() { Id = 1 }, new List<int>() { 1 });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : SetUserRolesAsync
        [Fact]
        public void SetUserRolesAsync_SetUserRolesAsyncNullUserEntityTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.SetUserRolesAsync(0, new List<int>());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepositorySetUserRolesAsync");
        }

        [Fact]
        public void SetUserRolesAsync_SetUserRolesAsyncDifferentQueriedRolesAndIdsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Permission>>()))
                .Returns(new List<Permission>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.SetUserRolesAsync(existingId, new List<int>() { 1 });

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
            Assert.Contains(result.Result.ErrorList, e => e == "SpecialEducationPlanning
.Business.Repository.RoleRepository SetUserRolesAsync");
        }

        [Fact]
        public void SetUserRolesAsync_SetUserRolesAsyncEqualQueriedRolesAndIdsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Permission>>()))
                .Returns(new List<Permission>() { new Permission() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SetUserRolesAsync(existingId, new List<int>() { 1 });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : GetRolesByNameAsync
        [Fact]
        public void GetRolesByNameAsync_GetRolesByNameAsyncRoleIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetRolesByNameAsync("roleName");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void GetRolesByNameAsync_GetRolesByNameAsyncRoleIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Role, RoleModel>(It.IsAny<Role>()))
                .Returns(new RoleModel());

            // Act
            var result = repository.GetRolesByNameAsync("roleName");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<RoleModel>>(result.Result);
        }
        #endregion

        #region Func : GetRolesFilteredAsync
        [Fact]
        public void GetRolesFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepositoryKey.Setup(er => er.Query(It.IsAny<RoleMaterializedRoleModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<RoleModel>(new List<RoleModel>()
                    {
                        new RoleModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetRolesFilteredAsync(searchModel);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IPagedQueryResult<RoleModel>>>(result.Result);
        }
        #endregion
    }
}
