using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Repository.ReleaseNoteRepository;
using SpecialEducationPlanning
.Business.Test;
using SpecialEducationPlanning
.Domain.Entity;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class ReleaseInfoRepositoryTest : BaseTest
    {
        private readonly int existingId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly ReleaseInfoRepository repository;

        public ReleaseInfoRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new ReleaseInfoRepository(
                this.LoggerFactory.CreateLogger<ReleaseInfoRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : SetReleaseInfoDocument
        [Fact]
        public void SetReleaseInfoDocument_SetReleaseInfoDocumentEntityIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<ReleaseInfo>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(ReleaseInfo));

            // Act
            var result = repository.SetReleaseInfoDocument(0, "path", "fileName");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void SetReleaseInfoDocument_SetReleaseInfoDocumentEntityIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<ReleaseInfo>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new ReleaseInfo());

            mockObjectMapper.Setup(m => m.Map<ReleaseInfo, ReleaseInfoModel>(It.IsAny<ReleaseInfo>()))
                .Returns(new ReleaseInfoModel());

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SetReleaseInfoDocument(existingId, "path", "fileName");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ReleaseInfoModel>>(result.Result);
        }
        #endregion

        #region Func : GetNewestReleaseInfoDocumentAsync
        [Fact]
        public void GetNewestReleaseInfoDocumentAsyncTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<ReleaseInfo>>()))
                .Returns(new List<ReleaseInfo>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<ReleaseInfo, ReleaseInfoModel>(It.IsAny<ReleaseInfo>()))
                .Returns(new ReleaseInfoModel());

            // Act
            var result = repository.GetNewestReleaseInfoDocumentAsync();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ReleaseInfoModel>>(result.Result);
        }
        #endregion

        #region Func : GetReleaseInfoAsync
        [Fact]
        public void GetReleaseInfoAsync_GetReleaseInfoAsyncReleaseInfoIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<ReleaseInfo>>()))
                .Returns(new List<ReleaseInfo>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetReleaseInfoAsync("version", "fusionVersion");

            // Assert
            Assert.Null(result.Result.Content);
        }

        [Fact]
        public void GetReleaseInfoAsync_GetReleaseInfoAsyncReleaseInfoIsNotNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<ReleaseInfo>>()))
                .Returns(new List<ReleaseInfo>() { new ReleaseInfo() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<ReleaseInfo, ReleaseInfoModel>(It.IsAny<ReleaseInfo>()))
                .Returns(new ReleaseInfoModel());

            // Act
            var result = repository.GetReleaseInfoAsync("version", "fusionVersion");

            // Assert
            Assert.NotNull(result.Result.Content);
            Assert.IsType<RepositoryResponse<ReleaseInfoModel>>(result.Result);
        }
        #endregion

        #region Func : GetReleaseInfoFilteredAsync
        [Fact]
        public void GetReleaseInfoFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<ReleaseInfoMaterializedReleaseInfoModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<ReleaseInfoModel>(new List<ReleaseInfoModel>()
                    {
                        new ReleaseInfoModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetReleaseInfoFilteredAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<ReleaseInfoModel>>>(result.Result);
        }
        #endregion
    }
}
