using System.Linq;
using System.Collections.Generic;

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


namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class SortingFilteringRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<ILogger<SortingFilteringRepository>> mockLogger;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly SortingFilteringRepository repository;

        public SortingFilteringRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new SortingFilteringRepository(
                LoggerFactory.CreateLogger<SortingFilteringRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetSortingFilteringOptionsByEntity
        [Fact]
        public void GetSortingFilteringOptionsByEntityTest()
        {
            //Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<SortingFiltering>>()))
                .Returns(new List<SortingFiltering> { new SortingFiltering() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<SortingFiltering>, IEnumerable<SortingFilteringModel>>(It.IsAny<IEnumerable<SortingFiltering>>()))
                .Returns(new List<SortingFilteringModel>());

            // Act
            var result = repository.GetSortingFilteringOptionsByEntity("entity");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<SortingFilteringModel>>>(result.Result);
        }
        #endregion
    }
}
