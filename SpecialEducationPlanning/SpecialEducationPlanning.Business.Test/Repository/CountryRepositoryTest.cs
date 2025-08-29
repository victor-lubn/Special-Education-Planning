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
    public class CountryRepositoryTest : BaseTest
    {
        private readonly int existingCountryId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly CountryRepository repository;

        public CountryRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new CountryRepository(
                this.LoggerFactory.CreateLogger<CountryRepository>(),
                mockEntityRepositoryKey.Object,   
                mockEntityRepository.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>())
            );
        }

        private CountryModel ExistingCountryModel
        {
            get
            {
                return new CountryModel()
                {
                    Id = 1,
                    KeyName = "KeyName",
                };
            }
        }

        #region Func: GetCountryRegions
        [Fact]
        public void GetCountryRegions_GetContryRegionsInExistingCountryTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Country>>()))
                .Returns(new List<Country>() { new Country { Id = existingCountryId } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Country, CountryModel>(It.IsAny<Country>()).Regions)
                .Returns(new List<RegionModel>() { new RegionModel() });

            // Act
            var result = repository.GetCountryRegions(existingCountryId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<RegionModel>>>(result.Result);
        }

        [Fact]
        public void GetCountryRegions_GetCountryRegionsNullCountryTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Country>>()))
                .Returns(new List<Country>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetCountryRegions(existingCountryId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }
        #endregion

        #region Fun: GetCountriesFiltered
        [Fact]
        public void GetCountriesFilteredTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<CountryMaterializedCountryModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<CountryModel>(new List<CountryModel>()
                    {
                        new CountryModel()
                        {
                            Id = existingCountryId
                        }
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetCountriesFilteredAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<CountryModel>>>(result.Result);
        }
        #endregion

        #region Func : GetDuplicatedCountry
        [Fact]
        public void GetDuplicatedCountry_GetDuplicatedCountryIsDuplicatedTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Country>>()))
                .Returns(new List<Country>() { new Country { Id = existingCountryId, KeyName = "KeyName" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Country, CountryModel>(It.IsAny<Country>()))
                .Returns(new CountryModel());

            // Act
            var result = repository.GetDuplicatedCountry(ExistingCountryModel);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityAlreadyExist Country already exists from GetDuplicatedCountry");
        }

        [Fact]
        public void GetDuplicatedCountry_GetDuplicatedCountryNotDuplicatedTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Country>>()))
                .Returns(new List<Country>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetDuplicatedCountry(ExistingCountryModel);

            // Assert
            Assert.Equal(0, result.Result.ErrorList.Count);
            Assert.IsType<RepositoryResponse<CountryModel>>(result.Result);
        }
        #endregion
    }
}
