using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class EndUserRepositoryTest : BaseTest
    {
        private readonly int existingAiepId = 1;
        private readonly int existingEndUserId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;

        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearchManagementService;

        private readonly EndUserRepository repository;

        public EndUserRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockAzureSearchManagementService = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);

            repository = new EndUserRepository(
                this.LoggerFactory.CreateLogger<EndUserRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockAzureSearchManagementService.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        private EndUser ExistingEndUserWithEndUserAiep
        {
            get
            {
                return new EndUser()
                {
                    EndUserAieps = new List<EndUserAiep>
                    {
                        new EndUserAiep() { AiepId = 1 }
                    }
                };
            }
        }

        private Plan ExistingPlanWithAiep
        {
            get
            {
                return new Plan()
                {
                    Project = new Project()
                    {
                        Aiep = new Aiep() { Id = existingAiepId }
                    },
                    EndUserId = 1
                };
            }
        }

        #region Func : CompareEndUsers
        [Fact]
        public void CompareEndUsersTest()
        {
            // Arrange

            // Act
            var result = repository.CompareEndUsers(new EndUserModel() { FirstName = "FirstName" }, new EndUserModel() { FirstName = "DifFistName" });

            // Assert
            Assert.IsType<Collection<EndUserDiffModel>>(result);
        }
        #endregion

        #region Func : FindExistingEndUser
        [Fact]
        public void FindExistingEndUser_FindExistingEndUserEndUserModelIsNullTest()
        {
            // Arrange

            // Act
            var result = repository.FindExistingEndUser(null);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void FindExistingEndUser_FindExistingEndUserEndUserNotExistingTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.FindExistingEndUser(new EndUserModel());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void FindExistingEndUser_FindExistingEndUserEndUserExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<IEnumerable<Plan>>()))
                .Returns(new List<PlanModel>());

            mockObjectMapper.Setup(m => m.Map<EndUser, EndUserModel>(It.IsAny<EndUser>()))
                .Returns(new EndUserModel());

            // Act
            var result = repository.FindExistingEndUser(new EndUserModel());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<EndUserModel>>(result.Result);
        }
        #endregion

        #region Func : GetOrCreateEndUserAssignAiep
        [Fact]
        public void GetOrCreateEndUserAssignAiep_GetOrCreateEndUserAssignAiepEndUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUser>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.GetOrCreateEndUserAssignAiep(new EndUserModel(), existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<EndUserModel>>(result.Result);
        }

        [Fact]
        public void GetOrCreateEndUserAssignAiep_GetOrCreateEndUserAssignAiepCannotAssignEndUserToAiepNullEndUserTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object)
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUser>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.GetOrCreateEndUserAssignAiep(new EndUserModel(), existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not found from GetOrCreateEndUserAssignAiep");
        }

        [Fact]
        public void GetOrCreateEndUserAssignAiep_GetOrCreateEndUserAssignAiepCannotAssignEndUserToAiepNullAiepTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUser>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.GetOrCreateEndUserAssignAiep(new EndUserModel(), existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not found from GetOrCreateEndUserAssignAiep");
        }

        [Fact]
        public void GetOrCreateEndUserAssignAiep_GetOrCreateEndUserAssignAiepCannotAssignEndUserToAiepNullEndUserAiepTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object)
                .Returns(new List<EndUser>() { ExistingEndUserWithEndUserAiep }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUser>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockUnitOfWork.Setup(un => un.Rollback()).Verifiable();

            // Act
            var result = repository.GetOrCreateEndUserAssignAiep(new EndUserModel(), existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Aiep not found from GetOrCreateEndUserAssignAiep");
        }

        [Fact]
        public void GetOrCreateEndUserAssignAiep_GetOrCreateEndUserAssignAiepAssignedAiepTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object)
                .Returns(new List<EndUser>() {
                    new EndUser() { EndUserAieps = new List<EndUserAiep> { new EndUserAiep() { AiepId = -1 } } }
                    }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>() { new Aiep() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUser>()))
                .Returns(new EndUser());

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<EndUserAiep>()))
                .Returns(new EndUserAiep());

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUser, EndUserModel>(It.IsAny<EndUser>()))
                .Returns(new EndUserModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.GetOrCreateEndUserAssignAiep(new EndUserModel(), existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<EndUserModel>>(result.Result);
        }
        #endregion

        #region Func : GetEndUserLatestUserAiepAsync
        [Fact]
        public void GetEndUserLatestUserAiepAsync_GetEndUserLatestUserAiepAsyncNullEndUser()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetEndUserLatestUserAiepAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetEndUserLatestUserAiepAsync_GetEndUserLatestUserAiepAsyncExistingEndUser()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { ExistingPlanWithAiep }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            // Act
            var result = repository.GetEndUserLatestUserAiepAsync(existingEndUserId);

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<AiepModel>>(result.Result);
        }
        #endregion

        #region Func : GetEndUserOwnOrLatestUserAiepAsync
        [Fact]
        public void GetEndUserOwnOrLatestUserAiep_GetEndUserOwnOrLatestUserAiepNullUserTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetEndUserOwnOrLatestUserAiepAsync(0, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetEndUserOwnOrLatestUserAiep_GetEndUserOwnOrLatestUserAiep_NotNullUserAiepExistsInUser_Test()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
               .Returns(new List<EndUser>() {
                   new EndUser()
                   {
                       EndUserAieps = new List<EndUserAiep>()
                       {
                           new EndUserAiep()
                           {
                               AiepId = existingAiepId
                           }
                       }
                   }
               }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { ExistingPlanWithAiep }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            // Act
            var result = repository.GetEndUserOwnOrLatestUserAiepAsync(existingEndUserId, existingAiepId);

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<AiepModel>>(result.Result);
        }

        [Fact]
        public void GetEndUserOwnOrLatestUserAiep_GetEndUserOwnOrLatestUserAiep_NotNullUserAiepDoesNotExistInUser_NullUser_Test()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
               .Returns(new List<EndUser>() {
                   new EndUser()
                   {
                       EndUserAieps = new List<EndUserAiep>()
                       {
                           new EndUserAiep()
                           {
                               AiepId = 0
                           }
                       }
                   }
               }.AsQueryable().BuildMock().Object)
               .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetEndUserOwnOrLatestUserAiepAsync(0, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetEndUserOwnOrLatestUserAiep_GetEndUserOwnOrLatestUserAiep_NotNullUserAiepDoesNotExistInUser_AiepNotFound_Test()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
               .Returns(new List<EndUser>() {
                   new EndUser()
                   {
                       EndUserAieps = new List<EndUserAiep>()
                       {
                           new EndUserAiep()
                           {
                               AiepId = 1
                           }
                       }
                   }
               }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetEndUserOwnOrLatestUserAiepAsync(existingEndUserId, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetEndUserOwnOrLatestUserAiep_GetEndUserOwnOrLatestUserAiep_NotNullUserAiepDoesNotExistInUser_AiepFound_Test()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
               .Returns(new List<EndUser>() {
                   new EndUser()
                   {
                       EndUserAieps = new List<EndUserAiep>()
                       {
                           new EndUserAiep()
                           {
                               AiepId = 0
                           }
                       }
                   }
               }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { ExistingPlanWithAiep }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            // Act
            var result = repository.GetEndUserOwnOrLatestUserAiepAsync(existingEndUserId, existingAiepId);

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<AiepModel>>(result.Result);
        }
        #endregion

        #region Func : GetEndUserByMandatoryFieldsAsyncS
        [Fact]
        public void GetEndUserByMandatoryFieldsAsync_GetEndUserByMandatoryFieldsAsyncEndUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            // Act
            var result = repository.GetEndUserByMandatoryFieldsAsync(new EndUserModel());

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetEndUserByMandatoryFieldsAsync_GetEndUserByMandatoryFieldsAsyncEndUserIsNotNull()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<EndUserModel, EndUser>(It.IsAny<EndUserModel>()))
                .Returns(new EndUser());

            mockObjectMapper.Setup(m => m.Map<EndUser, EndUserModel>(It.IsAny<EndUser>()))
                .Returns(new EndUserModel());

            // Act
            var result = repository.GetEndUserByMandatoryFieldsAsync(new EndUserModel());

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<EndUserModel>>(result.Result);
        }
        #endregion

        #region Func : EndUserCleanManagment
        [Fact]
        public void EndUserCleanManagment_EndUserCleanManagmentNullEndUserTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>().AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.EndUserCleanManagment();

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "GenericBusinessError Invalid EndUser from EndUserCleanManagment");
        }

        [Fact]
        public void EndUserCleanManagment_EndUserCleanManagmentNotNullEndUserTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.EndUserCleanManagment();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : CallIndexerAsync
        
        [Fact]
        public void CallIndexerAsyncTest()
        {
            // Arrange
            mockAzureSearchManagementService.Setup(az => az.MergeOrUploadDocuments(It.IsAny<IEnumerable<OmniSearchEndUserIndexModel>>()))
                .Verifiable();

            mockAzureSearchManagementService
                .Setup(az => az.GetDocuments<OmniSearchEndUserIndexModel, EndUser>(It.IsAny<List<EndUser>>()))
                .Returns(new List<OmniSearchEndUserIndexModel>());

            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<EndUser>>()))
                .Returns(new List<EndUser>() { new EndUser() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.CallIndexerAsync(1, 1, new DateTime(),0);

            // Assert
            Assert.Equal(TaskStatus.RanToCompletion, result.Status);
        }
        #endregion
    }
}

