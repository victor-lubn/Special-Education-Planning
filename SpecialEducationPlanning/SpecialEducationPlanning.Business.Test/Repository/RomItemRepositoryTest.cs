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
using Xunit;
using MockQueryable.Moq;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class RomItemRepositoryTest : BaseTest
    {
        private readonly int existingId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly RomItemRepository repository;

        public RomItemRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            //Behaviour is Default for now
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Default);

            repository = new RomItemRepository(
                LoggerFactory.CreateLogger<RomItemRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetRomItemByNameAsync
        [Fact]
        public void GetRomItemByNameAsync_GetRomItemByNameAsyncZeroRomItemsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<RomItem>>()))
                .Returns(new List<RomItem>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetRomItemByNameAsync("name");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
        }

        [Fact]
        public void GetRomItemByNameAsync_GetRomItemByNameAsyncRomItemExistsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<RomItem>>()))
                .Returns(new List<RomItem>() { new RomItem() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetRomItemByNameAsync("name");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<RomItem>>(result.Result);
        }
        #endregion

        #region Func CreateRomItem
        [Fact]
        public void CreateRomItem_CreateRomItemRomItemIsNullTest()
        {
            // Arrange

            // Act
            var result = repository.CreateRomItem(null, existingId, existingId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness");
        }


        [Fact]
        public void CreateRomItem_CreateRomItemVersionIdHasValueButNotFoundTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Version>>(), CancellationToken.None))
                .ReturnsAsync(false);

            // Act
            var result = repository.CreateRomItem(new RomItemModel(), existingId, existingId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateRomItem_CreateRomItemCatalogNotFoundTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Version>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Catalog>>(), CancellationToken.None))
                .ReturnsAsync(false);

            // Act
            var result = repository.CreateRomItem(new RomItemModel(), existingId, existingId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateRomItem_CreateRomItemCatalogFoundTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Version>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.AnyAsync(It.IsAny<ISpecification<Catalog>>(), CancellationToken.None))
                .ReturnsAsync(true);

            mockEntityRepository.Setup(er => er.Add(It.IsAny<RomItem>()))
                .Returns(new RomItem());

            mockObjectMapper.Setup(m => m.Map<RomItem, RomItemModel>(It.IsAny<RomItem>()))
                .Returns(new RomItemModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateRomItem(new RomItemModel(), 0, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<RomItemModel>>(result.Result);
        }
        #endregion

        #region Func : DeleteRomItemsFromVersion
        [Fact]
        public void DeleteRomItemsFromVersion_VersionRomItemsIsZeroTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<RomItem>>()))
                .Returns(new List<RomItem>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.DeleteRomItemsFromVersion(0);

            // Assert
            Assert.False(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound  from DeleteRomItemsFromVersion");
        }

        [Fact]
        public void DeleteRomItemsFromVersion_VersionRomItemsExistsAndItsHasValuesTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<RomItem>>()))
                .Returns(new List<RomItem>() { new RomItem() }.AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.Remove(It.IsAny<RomItem>())).Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit());

            // Act
            var result = repository.DeleteRomItemsFromVersion(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.True(result.Result.Content);
        }
        #endregion
    }
}
