using System.Linq;
using System.Collections.Generic;

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
    public class RegionRepositoryTest : BaseTest
    {
        private readonly int existingRegionId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly RegionRepository repository;
        public RegionRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new RegionRepository(
                this.LoggerFactory.CreateLogger<RegionRepository>(),
                mockEntityRepository.Object,
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>())
            );
        }

        private RegionModel ExistingRegionModel
        {
            get
            {
                return new RegionModel()
                {
                    Id = 1,
                    KeyName = "KeyName",
                };
            }
        }

        #region Func : GetRegionAreas
        [Fact]
        public void GetRegionAreas_GetRegionAreasInExistingRegionTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Region>>()))
                .Returns(new List<Region>() { new Region { Id = existingRegionId } }.AsQueryable().BuildMock().Object);
            mockObjectMapper.Setup(m => m.Map<Region, RegionModel>(It.IsAny<Region>()).Areas)
                .Returns(new List<AreaModel>() { new AreaModel() });
            // Act
            var result = repository.GetRegionAreas(existingRegionId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<AreaModel>>>(result.Result);
        }

        [Fact]
        public void GetRegionAreas_GetRegionAreasNullCountryTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Region>>()))
                .Returns(new List<Region>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetRegionAreas(existingRegionId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }
        #endregion

        #region Func : GetRegionsFiltered
        [Fact]
        public void GetRegionsFilteredTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<RegionMaterializedRegionModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<RegionModel>(new List<RegionModel>()
                {
                    new RegionModel()
                    {
                        Id = existingRegionId
                    }
                },
                take: null,
                skip: null,
                total: 1));

            // Act
            var result = repository.GetRegionsFilteredAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<RegionModel>>>(result.Result);
        }
        #endregion Func : GetDuplicatedRegion

        #region Func : GetDuplicatedRegion
        [Fact]
        public void GetDuplicatedRegion_GetDuplicatedRegionIsDuplicatedTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Region>>()))
                .Returns(new List<Region>() { new Region { Id = existingRegionId, KeyName = "KeyName" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Region, RegionModel>(It.IsAny<Region>()))
                .Returns(new RegionModel());
            // Act
            var result = repository.GetDuplicatedRegion(ExistingRegionModel);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist Region already exists from GetDuplicatedRegion");
        }

        [Fact]
        public void GetDuplicatedRegion_GetDuplicatedRegionNotDuplatedTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Region>>()))
                .Returns(new List<Region>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetDuplicatedRegion(ExistingRegionModel);

            // Assert
            Assert.Equal(0, result.Result.ErrorList.Count);
            Assert.IsType<RepositoryResponse<RegionModel>>(result.Result);
        }
        #endregion
    }
}
