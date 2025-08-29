using System.Linq;
using System.Collections.Generic;
using System.Threading;

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
.Business.Test;

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Service.Search;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class ProjectRepositoryTest : BaseTest
    {
        private readonly int existingProjectId = 1;
        private readonly int newAiepId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearch;
        private readonly Mock<IVersionRepository> mockVersionRepo;
        private readonly Mock<IHouseSpecificationRepository> mockHouseSpecification;

        private readonly ProjectRepository repository;

        public ProjectRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockHouseSpecification = new Mock<IHouseSpecificationRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockDbContextAccessor.Setup(m => m.GetCurrentContext())
                .Returns((DataContext)null);
            mockAzureSearch = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);
            mockVersionRepo = new Mock<IVersionRepository>(MockBehavior.Strict);

            repository = new ProjectRepository(
                this.LoggerFactory.CreateLogger<ProjectRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockAzureSearch.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object,
                mockHouseSpecification.Object,
                mockVersionRepo.Object
            );
        }

        private ProjectModel ExistingProjectModel
        {
            get
            {
                return new ProjectModel()
                {
                    Id = 1,
                };
            }
        }

        #region Func : CopyToAiep
        [Fact]
        public void CopyToAiepTest()
        {
            // Assert
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { new Project { Id = existingProjectId } }
                .AsQueryable()
                .BuildMock().Object);
            mockObjectMapper.Setup(m => m.Map<Project, ProjectModel>(It.IsAny<Project>()))
                .Returns(new ProjectModel());
            // Act
            var result = repository.CopyToAiep(ExistingProjectModel, newAiepId);

            // Arrange
            Assert.IsType<RepositoryResponse<ProjectModel>>(result.Result);
        }
        #endregion

        #region Func : GetProjectByProjectCode
        [Fact]
        public void GetProjectByProjectCode_GetProjectByProjectCodeProjectsAreNotNullTest()
        {
            // Assert
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { new Project() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(It.IsAny<Project[]>()))
                .Returns(new List<ProjectModel> { new ProjectModel() });

            // Act
            var result = repository.GetProjectsByProjectCode("AiepCode");

            // Arrange
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<ProjectModel>>>(result.Result);
        }

        [Fact]
        public void GetProjectByProjectCode_GetProjectByProjectCodeProjectsAreNullTest()
        {
            // Assert
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>().ToArray().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetProjectsByProjectCode("");

            // Arrange
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }
        #endregion

        #region Func : GetProjectPlans
        [Fact]
        public void GetProjectPlans_GetProjectPlansExistingProjectTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>() { new Project { Id = existingProjectId } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Plan, PlanModel>(It.IsAny<IEnumerable<Plan>>()))
                .Returns(new List<PlanModel> { new PlanModel() });

            // Act
            var result = repository.GetProjectPlans(existingProjectId);

            // Arrange
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<PlanModel>>>(result.Result);
        }

        [Fact]
        public void GetProjectPlans_GetProjectPlansNullProjectTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Project>>()))
                .Returns(new List<Project>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetProjectPlans(existingProjectId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }
        #endregion

        #region Func : CreateProjectForPlan
        [Fact]
        public void CreateProjectForPlan_CreateProjectForPlanAiepIsNullTest()
        {
            // Arrange

            // Act
            var result = repository.CreateProjectForPlan(new PlanModel(), 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "UndefinedAiep  from CreateProjectForPlan");
        }

        [Fact]
        public void CreateProjectForPlan_CreateProjectForPlanAiepNotEntityByIdSpecification()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Any(It.IsAny<ISpecification<Aiep>>()))
                .Returns(false);

            // Act
            var result = repository.CreateProjectForPlan(new PlanModel(), 1);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness  from CreateProjectForPlan");
        }

        [Fact]
        public void CreateProjectForPlan_CreateProjectForPlanAiepNewProject()
        {
            // Arrange

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mockUnitOfWork.Setup(un => un.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Project>()))
                 .Returns(new Project());

            mockObjectMapper.Setup(m => m.Map<ProjectModel, Project>(It.IsAny<ProjectModel>())).Returns(new Project());

            mockEntityRepository.Setup(er => er.Any(It.IsAny<ISpecification<Aiep>>()))
                .Returns(true);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Project>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Project());


            mockObjectMapper.Setup(m => m.Map<Project, ProjectModel>(It.IsAny<Project>()))
                .Returns(new ProjectModel());

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateProjectForPlan(new PlanModel { PlanCode = "PlanCode" }, 1);

            // Assert
            Assert.IsType<RepositoryResponse<ProjectModel>>(result.Result);
        }

        [Fact]
        public void CreateProjectForPlan_CreateProjectForPlanAiepNullEntity()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Any(It.IsAny<ISpecification<Aiep>>()))
                .Returns(true);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Project>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Project));

            mockObjectMapper.Setup(m => m.Map<Project, ProjectModel>(It.IsAny<Project>()))
            .Returns(new ProjectModel());

            mockObjectMapper.Setup(m => m.Map<ProjectModel, Project>(It.IsAny<ProjectModel>()))
            .Returns(new Project());

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mockUnitOfWork.Setup(un => un.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Project>()))
                 .Returns(new Project());



            // Act
            var result = repository.CreateProjectForPlan(new PlanModel { PlanCode = "PlanCode" }, 1);

            // Assert
            Assert.IsType<RepositoryResponse<ProjectModel>>(result.Result);
        }
        #endregion
    }
}
