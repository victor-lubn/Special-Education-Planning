using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using Xunit;
using MockQueryable.Moq;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class BuilderEducationerAiepRepositoryTest : BaseTest
    {
        private readonly int existingBuilderId = 1;
        private readonly int existingAiepId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly BuilderEducationerAiepRepository repository;

        public BuilderEducationerAiepRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new BuilderEducationerAiepRepository(
                this.LoggerFactory.CreateLogger<BuilderEducationerAiepRepository>(),
                mockEntityRepositoryKey.Object,
                mockEntityRepository.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>())
            );
        }

        #region Func : GetBuilderEducationerAiepModelRelation
        [Fact]
        public void GetBuilderEducationerAiepModelRelation_GetBuilderEducationerAiepModelRelationNonExistingBuilderEducationerAiep()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<BuilderEducationerAiep>>()))
                .Returns(new List<BuilderEducationerAiep>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void GetBuilderEducationerAiepModelRelation_GetBuilderEducationerAiepModelRelationExistingBuilderEducationerAiep()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<BuilderEducationerAiep>>()))
                .Returns(new List<BuilderEducationerAiep>() { new BuilderEducationerAiep() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderEducationerAiepModel>>(result.Result);
        }
        #endregion

        #region Func : CreateBuilderEducationerAiepModelRelation
        [Fact]
        public void CreateBuilderEducationerAiepModelRelation_CreateBuilderEducationerAiepModelRelationBuilderIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Aiep));

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(default(BuilderModel));

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(default(AiepModel));

            // Act
            var result = repository.CreateBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateBuilderEducationerAiepModelRelation_CreateBuilderEducationerAiepModelRelationAiepIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Aiep));

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(default(AiepModel));

            // Act
            var result = repository.CreateBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateBuilderEducationerAiepModelRelation_CreateBuilderEducationerAiepModelRelationBuilderAndAiepNotNullTest()
        {
            // Arrange

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Aiep());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<BuilderEducationerAiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new BuilderEducationerAiep());

            mockObjectMapper.Setup(m => m.Map<Aiep, AiepModel>(It.IsAny<Aiep>()))
                .Returns(new AiepModel());

            mockObjectMapper.Setup(m => m.Map<BuilderEducationerAiepModel, BuilderEducationerAiep>(It.IsAny<BuilderEducationerAiepModel>(), It.IsAny<BuilderEducationerAiep>()))
                .Returns(new BuilderEducationerAiep());

            mockObjectMapper.Setup(m => m.Map<BuilderEducationerAiep, BuilderEducationerAiepModel>(It.IsAny<BuilderEducationerAiep>()))
                .Returns(new BuilderEducationerAiepModel());

            mockObjectMapper.Setup(m => m.Map<BuilderEducationerAiepModel, BuilderEducationerAiep>(It.IsAny<BuilderEducationerAiepModel>()))
                .Returns(new BuilderEducationerAiep());

            mockUnitOfWork.Setup(un => un.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<BuilderEducationerAiep>()))
                 .Returns(new BuilderEducationerAiep());


            // Act
            var result = repository.CreateBuilderEducationerAiepModelRelation(existingBuilderId, existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderEducationerAiepModel>>(result.Result);
        }
        #endregion

        #region Func : GetBuilderEducationerAiepModelRelationByBuilderId
        [Fact]
        public void GetBuilderEducationerAiepModelRelationByBuilderIdTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<BuilderEducationerAiep>>()))
                .Returns(new List<BuilderEducationerAiep>() { new BuilderEducationerAiep() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderEducationerAiep, BuilderEducationerAiepModel>(It.IsAny<BuilderEducationerAiep>()))
                .Returns(new BuilderEducationerAiepModel());

            // Act
            var result = repository.GetBuilderEducationerAiepModelRelationByBuilderId(existingBuilderId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderEducationerAiepModel>>(result.Result);
        }
        #endregion

        #region Func : GetBuilderEducationerAiepModelByBuilderIdAiepId
        [Fact]
        public void GetBuilderEducationerAiepModelByBuilderIdAiepIdTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<BuilderEducationerAiep>>()))
                .Returns(new List<BuilderEducationerAiep>() { new BuilderEducationerAiep() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderEducationerAiep, BuilderEducationerAiepModel>(It.IsAny<BuilderEducationerAiep>()))
                .Returns(new BuilderEducationerAiepModel());

            // Act
            var result = repository.GetBuilderEducationerAiepModelByBuilderIdAiepId(existingBuilderId, existingAiepId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderEducationerAiepModel>>(result.Result);
        }
        #endregion
    }
}


