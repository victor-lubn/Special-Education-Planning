using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

using Moq;
using Xunit;
using MockQueryable.Moq;
using SpecialEducationPlanning
.Business.Test;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class CsvFileRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;

        private readonly CsvFileRepository repository;

        public CsvFileRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);

            repository = new CsvFileRepository(
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                this.LoggerFactory.CreateLogger<CsvFileRepository>()
            );
        }

        private UserCsvModel UserWithDepartment
        {
            get
            {
                return new UserCsvModel()
                {
                    departmentNumber = "1"
                };
            }
        }

        private UserCsvModel UserWithoutDepartment
        {
            get
            {
                return new UserCsvModel()
                {
                    departmentNumber = null
                };
            }
        }

        private AiepCsvModel AiepCsvModelCorrect
        {
            get
            {
                return new AiepCsvModel()
                {
                    AiepCode = "AiepCode",
                    Area = "Area",
                    DownloadLimit = "1",
                    DownloadSpeed = "1"
                };
            }
        }

        private AiepCsvModel AiepCsvModelNoDownload
        {
            get
            {
                return new AiepCsvModel()
                {
                    Area = "Area",
                    DownloadLimit = "NoLimit",
                    DownloadSpeed = "NoSpeed"
                };
            }
        }

        #region Func : InsertUsers
        [Fact]
        public void InsertUsers_InsertUsersExistingUserTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertUsers(new List<UserCsvModel>() { new UserCsvModel() });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist  from InsertUsers");
        }

        [Fact]
        public void InsertUsers_InsertUsersRepeatedUserTest()
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
                .Returns(new List<Role>() { new Role() { Name = "" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertUsers(new List<UserCsvModel>() { UserWithDepartment, UserWithDepartment });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist  from InsertUsers");
        }

        [Fact]
        public void InsertUsers_InsertUsersAiepIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<User>()))
                .Returns(new User());

            mockEntityRepositoryKey.Setup(er => er.GetAll<Role>())
                .Returns(new List<Role>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Role>>()))
                .Returns(new List<Role>() { new Role() { Name = "" } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertUsers(new List<UserCsvModel>() { UserWithoutDepartment });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "UndefinedAiep Aiep is null from InsertUsers");
            Assert.IsType<RepositoryResponse<int>>(result.Result);
        }

        [Fact]
        public void InsertUsers_InsertUsersAiepIsNotNullTest()
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
                .Returns(new List<Role>() { new Role() { Name = "" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<UserModel, User>(It.IsAny<UserModel>()))
                .Returns(new User());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.InsertUsers(new List<UserCsvModel>() { UserWithDepartment });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<int>>(result.Result);
        }
        #endregion

        #region Func : InsertAieps
        [Fact]
        public void InsertAieps_InsertAiepsAiepAlreadyExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertAieps(new List<AiepCsvModel>() { new AiepCsvModel() });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist  from InsertAieps");
        }

        [Fact]
        public void InsertAieps_InsertAiepsAiepRepeatedAiepTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
              .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>() { new Area() { Id = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep() { AiepCode = AiepCsvModelCorrect.AiepCode });

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertAieps(new List<AiepCsvModel>() { AiepCsvModelCorrect, AiepCsvModelCorrect });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist AiepCode from InsertAieps");
        }

        [Fact]
        public void InsertAieps_InsertAiepsNonExistingAreaTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
               .Returns(new List<Area>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertAieps(new List<AiepCsvModel>() { AiepCsvModelCorrect });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Area not found from InsertAieps");
        }

        [Fact]
        public void InsertAieps_InsertAiepsDownloadSpeedOrLimitIncorrectTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
               .Returns(new List<Area>() { new Area() { Id = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.InsertAieps(new List<AiepCsvModel>() { AiepCsvModelNoDownload });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness DownloadLimit or DownloadSpeed Error from InsertAieps");
        }

        [Fact]
        public void InsertAieps_InsertAiepsNonExistingAiepTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
               .Returns(new List<Area>() { new Area() { Id = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<User>>()))
                .Returns(new List<User>() { new User() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.InsertAieps(new List<AiepCsvModel>() { AiepCsvModelCorrect });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<int>>(result.Result);
        }
        #endregion
    }
}

