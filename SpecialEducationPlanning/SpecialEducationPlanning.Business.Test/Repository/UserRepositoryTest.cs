using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Domain.Specification.Search;
using Koa.Domain.Search.Page;

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
.Domain.Service.Search;
using SpecialEducationPlanning
.Business.Test;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;

using Moq;
using Xunit;
using MockQueryable.Moq;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;
using Shouldly;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class UserRepositoryTest : BaseTest
    {
        private readonly int existingUserId = 1;
        private readonly int existingAiepId = 1;
        private readonly int existingRoleId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearchManagementService;

        private readonly UserRepository repository;

        public UserRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockAzureSearchManagementService = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);

            repository = new UserRepository(
                LoggerFactory.CreateLogger<UserRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockAzureSearchManagementService.Object,
                new SpecificationBuilder(LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetUserByIdAsync
        [Fact]
        public void GetUserByIdAsync_GetUserByIdAsyncUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            // Act
            var result = repository.GetUserByIdAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound UserRepository: GetUserByIdAsync from GetUserByIdAsync");
        }

        [Fact]
        public void GetUserByIdAsync_GetUserByIdAsyncUserIsNotNull()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            // Act
            var result = repository.GetUserByIdAsync(existingUserId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result.Result);
        }
        #endregion

        #region Func : GetUserByEmailAsync

        [Fact]
        public async Task GetUserByEmailAync_UserIsFound_ReturnsUser()
        {
            //Arrange
            string email = "Test@Test.co.uk";

            List<User> usersReturned = new() 
            {
                new User()
                {
                    Id = 1,
                    UniqueIdentifier = email,
                    AiepId = 1
                }
            };

            UserModel userReturned = new()
            {
                Id = 1,
                UniqueIdentifier = email,
                AiepId = 1
            };

            mockEntityRepositoryKey.Setup(er => er.GetAll<User>()).Returns(usersReturned.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>())).Returns(userReturned);

            //Act
            RepositoryResponse<UserModel> result = await repository.GetUserByEmailAsync(email);

            //Assert
            result.ShouldNotBeNull();
            result.Content.ShouldNotBeNull();
            result.Content.ShouldBeOfType<UserModel>();

            mockEntityRepositoryKey.Reset();
            mockObjectMapper.Reset();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task GetUserByEmailAync_InvalidEmail_ReturnsNullContentWithError(string email)
        {
            //Arrange, Act
            RepositoryResponse<UserModel> result = await repository.GetUserByEmailAsync(email);

            //Assert
            result.ShouldNotBeNull();
            result.Content.ShouldBeNull();
            result.ErrorList.ShouldNotBeEmpty();
            result.ErrorList.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetUserByEmailAync_UserNotFound_ReturnsNullContentWithError()
        {
            //Arrange
            string email = "Test@Test.co.uk";

            List<User> usersReturned = new();

            mockEntityRepositoryKey.Setup(er => er.GetAll<User>()).Returns(usersReturned.AsQueryable().BuildMock().Object);

            //Act
            RepositoryResponse<UserModel> result = await repository.GetUserByEmailAsync(email);

            //Assert
            result.ShouldNotBeNull();
            result.Content.ShouldBeNull();
            result.ErrorList.ShouldNotBeEmpty();
            result.ErrorList.Count.ShouldBe(1);

            mockEntityRepositoryKey.Reset();
        }

        #endregion

        #region Func : GetUserWithRolesAsync
        [Fact]
        public void GetUserWithRolesAsync_GetUserWithRolesAsyncUserIsNull()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetUserWithRolesAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound UserRepository: GetUserWithRolesAsync from GetUserWithRolesAsync");
        }

        [Fact]
        public void GetUserWithRolesAsync_GetUserWithRolesAsyncUserIsNotNull()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<User, UserWithRoleModel>(It.IsAny<User>()))
                .Returns(new UserWithRoleModel());

            // Act
            var result = repository.GetUserWithRolesAsync(existingUserId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserWithRoleModel>>(result.Result);
        }
        #endregion

        #region Func : SetUserCurrentAiepId
        [Fact]
        public void SetUserCurrentAiepId_SetUserCurrentAiepIdUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.SetUserCurrentAiepId(0, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound User not found. from SetUserCurrentAiepId");
        }

        [Fact]
        public void SetUserCurrentAiepId_SetUserCurrentAiepIdAiepIdHasNoValueTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SetUserCurrentAiepId(existingUserId, null);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<object>>(result.Result);
        }

        [Fact]
        public void SetUserCurrentAiepId_SetUserCurrentAiepIdNoAiepWithAiepIdTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.SetUserCurrentAiepId(existingUserId, 0);

            // Arrange
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not found or user do not have access to it. from SetUserCurrentAiepId");
        }

        [Fact]
        public void SetUserCurrentAiepId_SetUserCurrentAiepIdAiepExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User()}.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SetUserCurrentAiepId(existingUserId, existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<object>>(result.Result);
        }
        #endregion

        #region Func : CreateUserFromCsvModel
        [Fact]
        public void CreateUserFromCsvModel_CreateUserFromCsvModelUserDoesNotExistDepartmentNumberIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.GetAll<Role>())
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<UserRole>()))
                .Returns(new UserRole());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.CreateUserFromCsvModel(new List<UserCsvModel>() { new UserCsvModel() { departmentNumber = null } });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "UndefinedAiep Aiep is null from CreateUserFromCsvModel");
        }

        [Fact]
        public void CreateUserFromCsvModel_CreateUserFromCsvModelUserDoesNotExistDepartmentNumberIsNotNullAiepDoesNotExistTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.GetAll<Role>())
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<UserRole>()))
                .Returns(new UserRole());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.CreateUserFromCsvModel(new List<UserCsvModel>() { new UserCsvModel() { departmentNumber = "1" } });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "UndefinedAiep Aiep is null from CreateUserFromCsvModel");
        }

        [Fact]
        public void CreateUserFromCsvModel_CreateUserFromCsvModelUserDoesNotExistAiepExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.GetAll<Role>())
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<UserRole>()))
                .Returns(new UserRole());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUserFromCsvModel(new List<UserCsvModel>() { new UserCsvModel() { departmentNumber = "1" } });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<int>>(result.Result);
        }

        [Fact]
        public void CreateUserFromCsvModel_CreateUserFromCsvModelUserDoesNotExistAlreadyAddedTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.GetAll<Role>())
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<UserRole>()))
                .Returns(new UserRole());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.CreateUserFromCsvModel(new List<UserCsvModel>() {
                new UserCsvModel() { userprincipalName = "user", departmentNumber = "1" },
                new UserCsvModel() { userprincipalName = "user", departmentNumber = "1" }
            });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist user from CreateUserFromCsvModel");
        }

        [Fact]
        public void CreateUserFromCsvModel_CreateUserFromCsvModelUserExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.CreateUserFromCsvModel(new List<UserCsvModel>() {
                new UserCsvModel() { userprincipalName = "user", departmentNumber = "1" }
            });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist user from CreateUserFromCsvModel");
        }
        #endregion

        #region Func : CreateUserWithRole
        [Fact]
        public void CreateUserWithRole_AiepDoesNotExistTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(false);

            // Act
            var result = repository.CreateUserWithRole(new UserModel() { AiepId = 0 }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not Found from CreateUserWithRole");
        }

        [Fact]
        public void CreateUserWithRole_CreateUserWithRoleUserAlreadyExistsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.CreateUserWithRole(new UserModel() { AiepId = existingAiepId }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist User already exists from CreateUserWithRole");
        }

        [Fact]
        public void CreateUserWithRole_CreateUserWithRoleRoleIdIsZeroTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
               .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUserWithRole(new UserModel() { AiepId = existingAiepId }, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result.Result);
        }

        [Fact]
        public void CreateUserWithRole_CreateUserWithRoleRoleIdIsNotZeroRoleIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
               .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Role));

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.CreateUserWithRole(new UserModel() { AiepId = existingAiepId }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Role not Found from CreateUserWithRole");
        }

        [Fact]
        public void CreateUserWithRole_CreateUserWithRoleRoleIdIsNotZeroRoleExistsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
               .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role());

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Role>()))
                .Returns(new Role());

            mockEntityRepository.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUserWithRole(new UserModel() { AiepId = existingAiepId }, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result.Result);
        }
        #endregion

        #region Func : UpdateUserWithRole
        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            // Act
            var result = repository.UpdateUserWithRole(new UserModel() { Id = 0 }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleAiepDoesNotExistTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(false);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role());

            // Act
            var result = repository.UpdateUserWithRole(new UserModel()
            {
                Id = existingUserId,
                AiepId = 0
            }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not Found from UpdateUserWithRole");
        }

        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleUserAlreadyExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role());

            // Act
            var result = repository.UpdateUserWithRole(new UserModel()
            {
                Id = existingUserId,
                AiepId = existingAiepId
            }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist User's unique identifier already exists from UpdateUserWithRole");
        }

        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleRoleIdIsZeroTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>()
                .AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserRole>>()))
                .Returns(new List<UserRole>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());
            mockObjectMapper.Setup(m => m.Map(It.IsAny<UserModel>(), It.IsAny<User>()))
           .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateUserWithRole(new UserModel()
            {
                Id = existingUserId,
                AiepId = existingAiepId
            }, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result.Result);
        }

        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleRoleIdNotZeroNullRoleTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>()
                .AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserRole>>()))
                .Returns(new List<UserRole>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Role));

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.UpdateUserWithRole(new UserModel()
            {
                Id = existingUserId,
                AiepId = existingAiepId
            }, existingRoleId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Role not Found from UpdateUserWithRole");
        }

        [Fact]
        public async void UpdateUserWithRole_UpdateUserWithRoleRoleIsNotNullUserRoleIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>()
                .AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<UserModel>(), It.IsAny<User>()))
         .Returns(new User());


            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserRole>>()))
                .Returns(new List<UserRole>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role { Name = "RoleName" });

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            var testUserModel = new UserModel
            {
                Id = existingUserId,
                AiepId = existingAiepId
            };

            // Act
            var result = await repository.UpdateUserWithRole(testUserModel, existingRoleId);

            // Assert
            Assert.Empty(result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result);
        }

        [Fact]
        public async void UpdateUserWithRole_UpdateUserWithRoleRoleIsNotNullDifferentUserRoleIdAndRoleIdTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>()
                .AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserRole>>()))
                .Returns(new List<UserRole>() { new UserRole() { RoleId = 1 } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<UserModel>(), It.IsAny<User>()))
         .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role { Name = "TestRoleName" });

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            var testUserModel = new UserModel
            {
                Id = existingUserId,
                AiepId = existingAiepId
            };

            // Act
            var result = await repository.UpdateUserWithRole(testUserModel, existingRoleId);

            // Assert
            Assert.Empty(result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result);
        }

        [Fact]
        public void UpdateUserWithRole_UpdateUserWithRoleRoleAndUserRollIsNotNullTest()
        {
            // Arrange

            mockEntityRepositoryKey.Setup(er => er.Add<UserRole>(It.IsAny<UserRole>()))
                .Returns(new UserRole());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Aiep>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>()
                .AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserRole>>()))
                .Returns(new List<UserRole>() { new UserRole() { RoleId = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role() { Name = "name" });

            mockEntityRepositoryKey.Setup(er => er.Remove<UserRole>(It.IsAny<int>()))
                .Verifiable();

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockObjectMapper.Setup(m => m.Map<User, UserModel>(It.IsAny<User>()))
                .Returns(new UserModel());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role() { Name = "name" });

            //IEntityRepository.Add<UserRole>(UserRole) 

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();


            // Act
            var result = repository.UpdateUserWithRole(new UserModel()
            {
                Id = existingUserId,
                AiepId = existingAiepId
            }, 2);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserModel>>(result.Result);
        }
        #endregion

        #region Func : UpdateEducationerAclAsync
        [Fact]
        public void UpdateEducationerAclAsync_UpdateEducationerAclAsyncEducationerIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.UpdateEducationerAclAsync(0);

            // Assert
            Assert.False(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound  from UpdateEducationerAclAsync");
        }
        #endregion

        #region Func : GetAllUsersWithRolesAsync
        [Fact]
        public void GetAllUsersWithRolesAsyncTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<User>())
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserWithRoleModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserWithRoleModel>());

            // Act
            var result = repository.GetAllUsersWithRolesAsync();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<UserWithRoleModel>>>(result.Result);
        }
        #endregion

        #region Func : GetAllUsersWithRolesAndPermissionsAsync
        [Fact]
        public void GetAllUsersWithRolesAndPermissionsAsyncTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<User>())
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserWithRoleAndPermissionsModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserWithRoleAndPermissionsModel>());

            // Act
            var result = repository.GetAllUsersWithRolesAndPermissionsAsync();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<UserWithRoleAndPermissionsModel>>>(result.Result);
        }
        #endregion

        #region Func : GetUsersWithRolesFilteredAsync
        [Fact]
        public void GetUsersWithRolesFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<UserMaterializedUserWithRolesModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<UserWithRoleModel>(new List<UserWithRoleModel>()
                    {
                        new UserWithRoleModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetUsersWithRolesFilteredAsync(searchModel);

            // Arrange
            Assert.IsType<RepositoryResponse<IPagedQueryResult<UserWithRoleModel>>>(result.Result);
        }
        #endregion

        #region Func : GetAllUsersByRoleId
        [Fact]
        public void GetAllUsersByRoleId_GetAllUsersByRoleIdRoleIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Role));

            // Act
            var result = repository.GetAllUsersByRoleId(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Role not Found from GetAllUsersByRoleId");
        }

        [Fact]
        public void GetAllUsersByRoleId_GetAllUsersByRoleIdRoleIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Role>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Role());

            mockEntityRepositoryKey.Setup(er => er.GetAll<User>())
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserModel>>(It.IsAny<IEnumerable<User>>()))
                .Returns(new List<UserModel>());

            // Act
            var result = repository.GetAllUsersByRoleId(existingRoleId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<UserModel>>>(result.Result);
        }
        #endregion

        #region Func : DeleteUserAsync
        [Fact]
        public void DeleteUserAsync_DeleteUserAsyncUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            // Act
            var result = repository.DeleteUserAsync(0);

            // Assert
            Assert.False(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound User not found from DeleteUserAsync");
        }
        #endregion

        #region Func : CallIndexerAsync
        [Fact]
        public void CallIndexerAsync_NoUpdateDateTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<User>())
                .Returns(new List<User>() {
                    new User(),
                    new User()
                }.AsQueryable().BuildMock().Object);

            mockAzureSearchManagementService.Setup(az => az.MergeOrUploadDocuments(It.IsAny<IEnumerable<OmniSearchUserIndexModel>>()))
                .Verifiable();

            mockAzureSearchManagementService
                .Setup(az => az.GetDocuments<OmniSearchUserIndexModel, User>(It.IsAny<List<User>>()))
                .Returns(new List<OmniSearchUserIndexModel>());

            // Act
            var result = repository.CallIndexerAsync(1, 1, null,0);

            // Assert
            Assert.Equal(1, 1);
        }
        #endregion
    }
}


