using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Domain.Specification.Search;
using Koa.Domain.Search.Page;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Test;
using SpecialEducationPlanning
.Domain.Specification.PlanSpecifications;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Api.Test.Support;

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class PlanRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepository;
        private readonly Mock<IEntityRepository> mockEntityRepository1;

        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearch;
        private readonly Mock<IPostCodeServiceFactory> mockPostCodeServiceFactory;
        private readonly Mock<ILogger<PlanRepository>> mockLogger;
        private readonly Mock<IAiepRepository> mockPlanRepo;

        private readonly PlanRepository repository;

        public PlanRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepository = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository1 = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            //Behavior Default
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockDbContextAccessor.Setup(a => a.GetCurrentContext()).Returns((DataContext)null);

            mockAzureSearch = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);
            mockPostCodeServiceFactory = new Mock<IPostCodeServiceFactory>(MockBehavior.Strict);
            mockLogger = new Mock<ILogger<PlanRepository>>();
            mockPlanRepo = new Mock<IAiepRepository>(MockBehavior.Strict);

            repository = new PlanRepository(
                mockLogger.Object,
                mockEntityRepository.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockAzureSearch.Object,
                mockPostCodeServiceFactory.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                new SqlAzureExecutionStrategy(),
                mockEntityRepository1.Object,
                mockPlanRepo.Object
            );
        }

        #region Func : GeneratePlanIdAsync

        #endregion

        #region Func : GetPlansByIdsAndTypeAsync

        #endregion

        #region Func : GeneratePlanIdAsync(DateTime)

        #endregion

        #region Func : GetPlanVersionsAsync
        [Fact]
        public void GetPlanVersionsAsyncTest()
        {
            // ArrangeZ
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<ICollection<Domain.Entity.Version>, ICollection<VersionModel>>(It.IsAny<ICollection<Domain.Entity.Version>>()))
                .Returns(new List<VersionModel>());


            // Act
            var result = repository.GetPlanVersionsAsync(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<VersionModel>>>(result.Result);
        }
        #endregion

        #region Func : GetPlansFilteredAsync
        [Fact(Skip = "cannot be unit tested due to async code")]
        public void GetPlansFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<PlanMaterializedPlanModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<PlanModel>(new List<PlanModel>()
                    {
                        new PlanModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetPlansFilteredAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<PlanModel>>>(result.Result);
        }
        #endregion

        #region Func : GetPlansByIdsAsync

        #endregion

        #region Func : GetPlansOmniSearchAsync

        #endregion

        #region Func : ApplyChangesPlanAsync

        [Fact]
        public async void ApplyChangesPlanAsync_Valid_Plan_Returns_PlanModel()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();
            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockEntityRepository1.Setup(er => er.Where(It.IsAny<Specification<Plan>>()))
                .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ActivePlanByIdSpecification>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            PlanModel model = new()
            {
                Id = 1
            };

            //Act
            var response = await repository.ApplyChangesPlanAsync(model);

            //Assert
            response.IsNotNull();
            response.GetType().Equals(typeof(PlanModel));
            response.ErrorList.Count.Equals(0);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository ApplyChangesPlanAsync call Commit", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel", times);
        }

        [Fact]
        public async void ApplyChangesPlanAsync_Invalid_Plan_Returns_PlanModel()
        {
            //Arrange
            Plan plan = null;
            PlanModel planModel = null;
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Never();

            var samplePlanEntity = new Plan()
            {
                Id = 1,
            };

            var samplePlanModel = new PlanModel()
            {
                Id = 1,
            };

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepository1.Setup(r => r.Where<Plan>(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan> { plan }.AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<EntityByIdSpecification<Plan>>()))
               .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(samplePlanModel);


            mockObjectMapper.Setup(m => m.Map<PlanModel, Plan>(It.IsAny<PlanModel>()))
                .Returns(plan);
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ActivePlanByIdSpecification>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            PlanModel model = new()
            {
                Id = 0
            };

            //Act
            var response = await repository.ApplyChangesPlanAsync(model);

            //Assert
            response.ShouldNotBeNull();
            response.Content.ShouldBeOfType(typeof(PlanModel));
            response.ErrorList.Count.ShouldBeGreaterThan(0);
            response.ErrorList.ShouldContain("GenericBusinessError");

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository ApplyChangesPlanAsync call Commit", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel", times);
        }

        [Fact]
        public async void ApplyChangesPlanAsync_Valid_Plan_and_Aiep_Returns_PlanModel()
        {
            //Arrange
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? times = Times.Once();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepository1.Setup(r => r.Where<Plan>(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, Plan>(It.IsAny<Plan>()))
                .Returns(new Plan());

            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ActivePlanByIdSpecification>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockEntityRepository1.Setup(er => er.Where(It.IsAny<EntityByIdSpecification<Plan>>()))
                .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            PlanModel model = new()
            {
                Id = 0
            };
            int AiepId = 0;

            //Act
            var response = await repository.ApplyChangesPlanAsync(model, AiepId);

            //Assert
            response.IsNotNull();
            response.GetType().Equals(typeof(PlanModel));
            response.ErrorList.Count.Equals(0);

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository ApplyChangesPlanAsync call Commit", times);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel", times);
        }

        [Fact]
        public async void ApplyChangesPlanAsync_Invalid_Plan_or_Aiep_Returns_PlanModel()
        {
            //Arrange
            Plan plan = null;
            PlanModel planModel = null;
            this.mockLogger.MockLog(LogLevel.Debug);
            Times? timesOnce = Times.Once();
            Times? timesNever = Times.Never();

            var samplePlanEntity = new Plan()
            {
                Id = 1,
            };

            var samplePlanModel = new PlanModel()
            {
                Id = 1,
            };

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();
            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            mockEntityRepository1.Setup(r => r.Where<Plan>(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan> { plan }.AsQueryable().BuildMock().Object);

            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ActivePlanByIdSpecification>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<EntityByIdSpecification<Plan>>()))
               .Returns(new List<Plan> { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(samplePlanModel);


            mockObjectMapper.Setup(m => m.Map<PlanModel, Plan>(It.IsAny<PlanModel>()))
                .Returns(plan);

           mockEntityRepository1.Setup(er => er.Add(It.IsAny<Plan>()))
           .Returns(plan);

            PlanModel model = new()
            {
                Id = 1
            };
            int AiepId = 1;

            //Act
            var response = await repository.ApplyChangesPlanAsync(model, AiepId);

            //Assert
            response.ShouldNotBeNull();
            response.Content.ShouldBeOfType(typeof(PlanModel));
            response.ErrorList.Count.ShouldBeGreaterThan(0);
            response.ErrorList.ShouldContain("GenericBusinessError");

            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository end call ApplyChangesPlanAsync -> return Repository response PlanModel Errors or Null plan", timesOnce);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository ApplyChangesPlanAsync call Commit", timesNever);
            this.mockLogger.VerifyLogger(LogLevel.Debug, "PlanRepository end call ApplyChangesPlanAsync -> Repository response PlanModel", timesNever);
        }

        #endregion

        #region Func : ChangePlanStateAsync
        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_IncludedArchive_NullPlan_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ChangePlanStateAsync(0, PlanState.Active, true);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Plan not found from ChangePlanStateAsync");
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_IncludedArchive_PlanIsStarredAndPlanStateArchived_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = true } }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Archived, true);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ActionNotAllowed Plan Is Starred from ChangePlanStateAsync");
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_NotIncludedArchive_NullPlan_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ChangePlanStateAsync(0, PlanState.Active, false);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Plan not found from ChangePlanStateAsync");
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_NotIncludedArchive_PlanIsStarredAndPlanStateArchived_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = true } }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Archived, false);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ActionNotAllowed Plan Is Starred from ChangePlanStateAsync");
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_IncludedArchive_PlanIsNotStarredAndPlanStateArchived_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = false } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Archived, true);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_IncludedArchive_PlanIsStarredAndPlanStateActive_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = true } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Active, true);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_IncludedArchive_PlanIsNotStarredAndPlanStateActive_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = false } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Active, true);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_NotIncludedArchive_PlanIsNotStarredAndPlanStateArchived_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = false } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Archived, false);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_NotIncludedArchive_PlanIsStarredAndPlanStateActive_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = true } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Active, false);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsync_NotIncludedArchive_PlanIsNotStarredAndPlanStateActive_Test()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() { Id = 1, IsStarred = false } }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(1, PlanState.Active, false);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : DeleteEmptyPlans
        [Fact]
        public void DeleteEmptyPlansTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockEntityRepository1.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Domain.Entity.Version>>(), CancellationToken.None))
                .ReturnsAsync(false);

            mockEntityRepository.Setup(er => er.Remove<Plan>(It.IsAny<int>())).Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.DeleteEmptyPlans();

            // Assert
            Assert.True(result.Result);
        }
        #endregion

        #region Func : ChangePlanStateAsync(PlanModel, PlanState)
        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsyncPlanStateActiveAndIsStarredTest()
        {
            // Arrange

            // Act
            var result = repository.ChangePlanStateAsync(new PlanModel() { IsStarred = true }, PlanState.Archived);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ActionNotAllowed");
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsyncPlanStateNotActiveNotStarredTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.FindOne<Plan>(It.IsAny<int>()))
                .Returns(new Plan());

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(new PlanModel() { IsStarred = false }, PlanState.Archived);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
        }

        [Fact]
        public void ChangePlanStateAsync_ChangePlanStateAsyncPlanStateActiveNotStarredTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.FindOne<Plan>(It.IsAny<int>()))
                .Returns(new Plan());

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ChangePlanStateAsync(new PlanModel() { IsStarred = false }, PlanState.Active);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
        }
        #endregion

        #region Func : AssignBuilderToPlan
        [Fact]
        public void AssignBuilderToPlanTest()
        {
            mockEntityRepository.Setup(er => er.FindOne<Plan>(It.IsAny<int>()))
                .Returns(new Plan());

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.AssignBuilderToPlan(1, new BuilderModel());

            // Assert
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
            Assert.Empty(result.Result.ErrorList);
        }
        #endregion

        #region Func : UnassignBuilderFromPlan
        [Fact]
        public void UnassignBuilderFromPlanTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UnassignBuilderFromPlan(1);

            // Assert
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
            Assert.Empty(result.Result.ErrorList);
        }
        #endregion

        #region Func : GetAllPlansWithoutArchivedPlansAsync
        [Fact]
        public void GetAllPlansWithoutArchivedPlansAsyncTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(It.IsAny<IEnumerable<Plan>>()))
                .Returns(new List<PlanModel>());

            // Act
            var result = repository.GetAllPlansWithoutArchivedPlansAsync();

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<PlanModel>>>(result.Result);
            Assert.Empty(result.Result.ErrorList);
        }
        #endregion

        #region Func : GetAllPlansWithArchivedPlansAsync
        [Fact]
        public void GetAllPlansWithArchivedPlansAsyncTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(It.IsAny<IEnumerable<Plan>>()))
                .Returns(new List<PlanModel>() { new PlanModel() });

            // Act
            var result = repository.GetAllPlansWithArchivedPlansAsync();

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<PlanModel>>>(result.Result);
            Assert.Empty(result.Result.ErrorList);
        }
        #endregion

        #region Func : GetAllArchivedPlansAsync
        [Fact]
        public void GetAllArchivedPlansAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository1.Setup(er => er.Query(It.IsAny<PlanMaterializedPlanModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<PlanModel>(new List<PlanModel>()
                    {
                        new PlanModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetAllArchivedPlansAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<PlanModel>>>(result.Result);
        }
        #endregion

        #region Func : FindOnePlanWhithoutArchivedPlansAsync
        [Fact]
        public void FindOnePlanWhithoutArchivedPlansAsyncTest()
        {
            // Assert
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            // Act
            var result = repository.FindOnePlanWhithoutArchivedPlansAsync(1);

            // Assert
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
            Assert.Empty(result.Result.ErrorList);
        }
        #endregion

        #region Func : AutomaticArchive(int)

        [Fact]
        public void AutomaticArchiveTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.AutomaticArchive(100);

            // Assert

        }
        #endregion

        #region Func : AutomaticArchive(DateTime, int, int)

        [Fact]
        public void AutomaticArchive2Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.AutomaticArchive(100);

            // Assert

        }
        #endregion

        #region Func : AutomaticDeletion(double)

        #endregion

        #region Func : AutomaticDeletion(double, DateTime, int)

        #endregion

        #region Func : AssignBuilder
        [Fact]
        public void AssignBuilderTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.AssignBuilder(1, 1);

            // Assert
            Assert.IsType<PlanModel>(result.Result);
        }
        #endregion

        #region Func : AssignPlanToAiepAsync

        #endregion

        #region Func : TransferSinglePlanBetweenAieps
        [Fact]
        public void TransferSinglePlanBetweenAiepsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>()
                { new Plan()
                    {
                        Project = new Project() { AiepId = 1 }
                    }
                }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.TransferSinglePlanBetweenAieps(1, 10);

            // Assert
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
            //Assert.Equal(10, result.Result.Content.Project.AiepId);
        }
        #endregion

        #region Func : TransferMultiplePlanToUnassignedBuilder
        [Fact]
        public void TransferMultiplePlanToUnassignedBuilderTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>()
                { new Plan()
                    {
                        Project = new Project() { AiepId = 1 }
                    }
                }.AsQueryable().BuildMock().Object);

            // Assuming mockEntityRepositoryKey is your mock object
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<Specification<Plan>>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);


            mockObjectMapper.Setup(m => m.Map<IEnumerable<Plan>, IEnumerable<PlanModel>>(It.IsAny<IEnumerable<Plan>>()))
                .Returns(new List<PlanModel>());



            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.TransferMultiplePlanToUnassignedBuilder(1, 1);

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<PlanModel>>>(result.Result);
        }
        #endregion

        #region Func : CopyToProject
        [Fact]
        public void CopyToProjectTest()
        {
            // Arrange

            // Act
            var result = repository.CopyToProject(new PlanModel(), 1);

            // Assert
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
        }
        #endregion

        #region Func : GetPlanWithVersions
        [Fact]
        public void GetPlanWithVersions_GetPlanWithVersionsNullPlanTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetPlanWithVersions(1);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound  from GetPlanWithVersions");
        }

        [Fact]
        public void GetPlanWithVersions_GetPlanWithVersionsNoNullPlanTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
                .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<Plan>()))
                .Returns(new PlanModel());

            // Act
            var result = repository.GetPlanWithVersions(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<PlanModel>>(result.Result);
        }
        #endregion

        #region Func : FindOneWithEducationerAsync
        [Fact]
        public void FindOneWithEducationerAsyncTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.Where(It.IsAny<ISpecification<Plan>>()))
               .Returns(new List<Plan>() { new Plan() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel2>(It.IsAny<Plan>()))
                .Returns(new PlanModel2());

            // Act
            var result = repository.FindOneWithEducationerAsync(1);

            // Assert
            Assert.IsType<PlanModel2>(result.Result);
        }
        #endregion

        #region Func : UpdateBuilderPlansTradingName
        [Fact]
        public void UpdateBuilderPlansTradingNameTest()
        {
            // Arrange
            mockEntityRepository1.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>()
                    {
                        new Plan() { BuilderId = 1, BuilderTradingName = "NotSame" },
                        new Plan() { BuilderId = 1, BuilderTradingName = "Same" }
                    }.AsQueryable().BuildMock().Object);

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            mockUnitOfWork.Setup(un => un.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = repository.UpdateBuilderPlansTradingName(1, "Same");

            // Assert
            Assert.True(result.Result);
        }
        #endregion
    }
}


