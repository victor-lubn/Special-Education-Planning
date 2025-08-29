using System.Linq;
using System.Collections.Generic;
using System.Threading;

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
    public class AiepRepositoryTest : BaseTest
    {
        private readonly int existingBuilderId = 1;
        private readonly int existingAiepId = 1;
        private readonly int existingAreaId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly AiepRepository repository;

        public AiepRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new AiepRepository(
                this.LoggerFactory.CreateLogger<AiepRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        private AiepModel ExistingAiepModel
        {
            get
            {
                return new AiepModel()
                {
                    Id = 1,
                    AiepCode = "AiepCode",
                };
            }
        }

        private AiepModel NonExistingAiepModel
        {
            get
            {
                return new AiepModel()
                {
                    Id = 0,
                    AiepCode = "AiepCode",
                };
            }
        }

        #region Func : CheckBuilderInAiepAsync
        [Fact]
        public void CheckBuilderInAiepAsyncTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<BuilderEducationerAiep>>()))
                .Returns(new List<BuilderEducationerAiep>() { new BuilderEducationerAiep { Id = existingBuilderId } }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.CheckBuilderInAiepAsync(existingAiepId, existingBuilderId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<bool>>(result.Result);
        }
        #endregion

        #region Func : GetAllIdsIgnoreAcl
        [Fact]
        public void GetAllIdsIgnoreAclTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.GetAll<Aiep>())
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetAllIdsIgnoreAcl();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<int>>>(result.Result);
        }
        #endregion

        #region Func : GetAiepProjectsAsync
        [Fact]
        public void GetAiepProjectsAsync_GetAiepProjectsAsyncAiepIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetAiepProjectsAsync(existingAiepId);

            //Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void GetAiepProjectsAsync_GetAiepProjectsAsyncAiepNotNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>() { new Aiep { Id = existingAiepId } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()).Projects)
                .Returns(new List<ProjectModel> { new ProjectModel() });

            // Act
            var result = repository.GetAiepProjectsAsync(existingAiepId);

            //Assert
            Assert.Equal(0, result.Result.ErrorList.Count);
            Assert.IsType<RepositoryResponse<ICollection<ProjectModel>>>(result.Result);
        }
        #endregion

        // To do
        #region Func : UpdateAllAiepsAclAsync
        // UpdateAllAiepAclAsyncTest
        #endregion

        // To do
        #region Func : UpdateAiepAclAsync
        // UpdateAllAiepAclAsyncTest
        #endregion

        #region Func : GetAiepByCode
        [Fact]
        public void GetAiepByCode_GetAiepByCodeAiepIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
               .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetAiepByCode("AiepCode");

            // Arrange
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound  from GetAiepByCode");
        }

        [Fact]
        public void GetAiepByCode_GetAiepByCodeAiepIsNotNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                            .Returns(new List<Aiep>() { new Aiep { Id = existingAiepId } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()).Projects)
                .Returns(new List<ProjectModel> { new ProjectModel() });

            // Act
            var result = repository.GetAiepByCode("AiepCode");

            // Arrange
            Assert.Equal(0, result.Result.ErrorList.Count);
            Assert.IsType<RepositoryResponse<AiepModel>>(result.Result);
        }
        #endregion

        #region Func : GetAiepsFilteredAsync
        [Fact]
        public void GetAiepsFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<AiepMaterializedAiepModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<AiepModel>(new List<AiepModel>()
                {
                    new AiepModel()
                    {
                        Id = existingAiepId
                    }
                },
                take: null,
                skip: null,
                total: 1));

            // Act
            var result = repository.GetAiepsFilteredAsync(searchModel);

            // Arrange
            Assert.IsType<RepositoryResponse<IPagedQueryResult<AiepModel>>>(result.Result);
        }
        #endregion

        #region Func : GetAllAiepsByAreaAsync
        [Fact]
        public void GetAllAiepsByAreaAsync_GetAllAiepsByAreaAsyncAreaIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Area>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Area());

            mockEntityRepository.Setup(er => er.GetAll<Aiep>())
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Aiep>, IEnumerable<AiepModel>>(It.IsAny<IEnumerable<Aiep>>()))
                .Returns(new List<AiepModel>() { new AiepModel() });

            // Act
            var result = repository.GetAllAiepsByAreaAsync(existingAreaId);

            //Arrange
            Assert.Equal(0, result.Result.ErrorList.Count);
            Assert.IsType<RepositoryResponse<AiepAreaModel>>(result.Result);
        }

        [Fact]
        public void GetAllAiepsByAreaAsync_GetAllAiepsByAreaAsyncAreaIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Area>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Area));

            mockEntityRepositoryKey.Setup(er => er.GetAll<Aiep>())
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Aiep>, IEnumerable<AiepModel>>(It.IsAny<IEnumerable<Aiep>>()))
                .Returns(new List<AiepModel>() { new AiepModel() });

            // Act
            var result = repository.GetAllAiepsByAreaAsync(existingAreaId);

            //Arrange
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Area not Found from GetAllAiepsByAreaAsync");
        }
        #endregion

        #region Func : CreateUpdateAiepAsync
        [Fact]
        public void CreateUpdateAiepAsync_CreateUpdateAiepAsyncAiepNonExistingAiepModel()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUpdateAiepAsync(NonExistingAiepModel);

            // Assert
            Assert.IsType<AiepModel>(result.Result);
        }

        [Fact]
        public void CreateUpdateAiepAsync_CreateUpdateAiepAsyncAiepExistingAiepModel()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Aiep>>()))
                .Returns(new List<Aiep>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<AiepModel, Aiep>(It.IsAny<AiepModel>(), It.IsAny<Aiep>()))
                .Returns(new Aiep());

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUpdateAiepAsync(ExistingAiepModel);

            // Assert
            Assert.IsType<AiepModel>(result.Result);
        }
        #endregion
    }
}


