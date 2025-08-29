using System;
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
.Domain.Service.Search;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Business.DtoModel.BuilderSapSearch;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using Koa.Domain.Search.Page;
using SpecialEducationPlanning
.Domain.Specification.BuilderSpecifications;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class BuilderRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IAzureSearchManagementService> mockAzureSearch;
        private readonly BuilderRepository repository;

        public BuilderRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockAzureSearch = new Mock<IAzureSearchManagementService>(MockBehavior.Strict);

            repository = new BuilderRepository(
                this.LoggerFactory.CreateLogger<BuilderRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockAzureSearch.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetExistingBuilderAsync
        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_NoMandatoryFieldsError_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            var testInputBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "PostCode"
            };

            var testOutputBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "PostCode",
                CreationUser = "John Doe"
            };

            var testBuilder = new Builder
            {
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "PostCode",
                CreationUser = "John Doe"
            };

            mockEntityRepository.Setup(rep => rep.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { testBuilder }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(mapper => mapper.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(testOutputBuilderModel);

            // Act
            var result = await repository.GetExistingBuilderAsync(testInputBuilderModel);

            // Assert
            Assert.Equal(0, result.ErrorList.Count);
            Assert.Equal(typeof(BuilderModel), result.Content.GetType());
            Assert.Equal("John Doe", result.Content.CreationUser);
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAddress1_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = "PostCode"
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = "PostCode"
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAdress1AndTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = null,
                Postcode = "Postcode"
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAdress1AndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderAsync_GetExistingBuilderAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullTradingNameAndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public void GetExistingBuilderAsync_GetExistingBuilderAsync_NotNullAccountNumber_NullBuilder_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetExistingBuilderAsync(new BuilderModel()
            {
                AccountNumber = "AccountNumber",
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "Postcode"
            });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }

        [Fact]
        public void GetExistingBuilderAsync_GetExistingBuilderAsync_NotNullAccountNumber_NotNullBuilder_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            // Act
            var result = repository.GetExistingBuilderAsync(new BuilderModel()
            {
                AccountNumber = "AccountNumber",
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "Postcode"
            });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : GetBuildersByIdsAsync
        [Fact]
        public void GetBuildersByIdsAsync_GetBuildersByIdsAsyncAzureSortIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IList<Builder>, IList<BuilderModel>>(It.IsAny<IList<Builder>>(), It.IsAny<IList<BuilderModel>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetBuildersByIdsAsync(new List<int>(), 0, 0, 0, null);

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<BuilderModel>>>(result.Result);
        }

        [Fact]
        public void GetBuildersByIdsAsync_GetBuildersByIdsAsyncAzureSortIsDescendingTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
               .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IList<Builder>, IList<BuilderModel>>(It.IsAny<IList<Builder>>(), It.IsAny<IList<BuilderModel>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetBuildersByIdsAsync(new List<int>(), 0, 0, 0, new Koa.Domain.Search.Sort.SortDescriptor() { Member = "TradingName", Direction = Koa.Domain.Search.Sort.SortDirection.Descending });

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<BuilderModel>>>(result.Result);
        }

        [Fact]
        public void GetBuildersByIdsAsync_GetBuildersByIdsAsyncAzureSortIsNotDescendingTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
               .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IList<Builder>, IList<BuilderModel>>(It.IsAny<IList<Builder>>(), It.IsAny<IList<BuilderModel>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetBuildersByIdsAsync(new List<int>(), 0, 0, 0, new Koa.Domain.Search.Sort.SortDescriptor() { Member = "TradingName", Direction = Koa.Domain.Search.Sort.SortDirection.Ascending });

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<BuilderModel>>>(result.Result);
        }
        #endregion

        #region Func : GetExistingBuilderOrEmptyAsync
        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "PostCode"
            };

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Empty(result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result);
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAddress1_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = "PostCode"
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = "PostCode"
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAdress1AndTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = null,
                Postcode = "Postcode"
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullAdress1AndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NullAccountNumber_AccountNumberError_MandatoryFieldsError_NullTradingNameAndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = null
            };

            // Act
            var result = await repository.GetExistingBuilderOrEmptyAsync(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public void GetExistingBuilderOrEmptyAsync_GetExistingBuilderOrEmptyAsync_NotNullAccountNumber_NullBuilder_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetExistingBuilderOrEmptyAsync(new BuilderModel()
            {
                AccountNumber = "AccountNumber",
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "PostCode"
            });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.Null(result.Result.Content);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : GetPosibleTdpMatchingBuilders
        [Fact]
        public void GetPosibleTdpMatchingBuilders_GetPosibleTdpMatchingBuildersNullBuilderNullBuildersListTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(r => r.Where(It.IsAny<BuilderByMandatoryFieldsSpecification>()))
            .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>());

            // Act
            var result = repository.GetPosibleTdpMatchingBuilders(new BuilderModel());

            // Assert
            Assert.Empty(result.Result.Content.Builders);
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }

        [Fact]
        public void GetPosibleTdpMatchingBuilders_GetPosibleTdpMatchingBuildersNullBuilderNotNullBuildersListTest()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object)
                .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);


            mockEntityRepositoryKey.Setup(r => r.Where(It.IsAny<BuilderByMandatoryFieldsSpecification>()))
            .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);


            mockObjectMapper.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>());


            // Act
            var result = repository.GetPosibleTdpMatchingBuilders(new BuilderModel());

            // Assert
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }

        [Fact]
        public void GetPosibleTdpMatchingBuilders_GetPosibleTdpMatchingBuildersNotNullBuilder_SameIds_NullBuildersListTest()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object)
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);


            mockEntityRepositoryKey.Setup(r => r.Where(It.IsAny<BuilderByMandatoryFieldsSpecification>()))
            .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>());

            // Act
            var result = repository.GetPosibleTdpMatchingBuilders(new BuilderModel());

            // Assert
            Assert.Empty(result.Result.Content.Builders);
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }

        [Fact]
        public void GetPosibleTdpMatchingBuilders_GetPosibleTdpMatchingBuildersNotNullBuilder_SameIds_NotNullBuildersListTest()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object)
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>());

            mockEntityRepositoryKey.Setup(r => r.Where(It.IsAny<BuilderByMandatoryFieldsSpecification>()))
                .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);


            // Act
            var result = repository.GetPosibleTdpMatchingBuilders(new BuilderModel());

            // Assert
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }

        [Fact]
        public void GetPosibleTdpMatchingBuilders_GetPosibleTdpMatchingBuildersNotNullBuilderDifferentIdsTest()
        {
            // Arrange
            mockEntityRepositoryKey.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object)
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<BuilderModel, Builder>(It.IsAny<BuilderModel>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            // Act
            var result = repository.GetPosibleTdpMatchingBuilders(new BuilderModel() { Id = 1 });

            // Assert
            Assert.NotEmpty(result.Result.Content.Builders);
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }
        #endregion

        #region Func : GetPosibleBuildersMatch
        [Fact]
        public void GetPosibleBuildersMatchTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Builder>, IEnumerable<BuilderModel>>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetPosibleBuildersMatch(new BuilderModel());

            // Assert
            Assert.IsType<RepositoryResponse<IEnumerable<BuilderModel>>>(result.Result);

        }
        #endregion

        #region Func : GetPossibleTdpMatchingBuilderByAccountNumberAsync
        [Fact]
        public void GetPossibleTdpMatchingBuilderByAccountNumberAsync_GetPossibleTdpMatchingBuilderByAccountNumberAsyncExactBuilderIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetPossibleTdpMatchingBuilderByAccountNumberAsync("accountNumber");

            // Assert
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }

        [Fact]
        public void GetPossibleTdpMatchingBuilderByAccountNumberAsync_GetPossibleTdpMatchingBuilderByAccountNumberAsyncExactBuilderIsNotNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            // Act
            var result = repository.GetPossibleTdpMatchingBuilderByAccountNumberAsync("accountNumber");

            // Assert
            Assert.IsType<RepositoryResponse<ValidationBuilderModel>>(result.Result);
        }
        #endregion

        #region Func : ValidateEducationerAiep
        [Fact]
        public void ValidateEducationerAiep_ValidateEducationerAiepEducationerIsNullTest()
        {
            // Arrange

            Builder builder = null;

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(builder);

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Aiep());

            // Act
            var result = repository.ValidateEducationerAiep(0, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void ValidateEducationerAiep_ValidateEducationerAiepAiepIsNullTest()
        {

            Aiep Aiep = null;
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Builder());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(Aiep);

            // Act
            var result = repository.ValidateEducationerAiep(1, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void ValidateEducationerAiep_ValidateEducationerAiepEntitiesFoundTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Aiep());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Builder());

            // Act
            var result = repository.ValidateEducationerAiep(1, 1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : CreateAsync
        [Fact]
        public void CreateAsync_CreateAsyncEducationerAndAiepAreNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Aiep));

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.CreateAsync(new BuilderModel(), 0, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateAsync_CreateAsyncEducationerIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(User));

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Aiep());

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.CreateAsync(new BuilderModel(), 0, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateAsync_CreateAsyncAiepIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Aiep));

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.CreateAsync(new BuilderModel(), 0, 0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void CreateAsync_CreateAsyncEducationerAndAiepExistTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User());

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Aiep());

            mockEntityRepository.Setup(er => er.Add(It.IsAny<Builder>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateAsync(new BuilderModel(), 0, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : DeleteAccountNumberAsync
        [Fact]
        public void DeleteAccountNumberAsync_DeleteAccountNumberAsyncBuilderIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Builder));

            // Act
            var result = repository.DeleteAccountNumberAsync(0, "AccountNumber");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness  from DeleteAccountNumberAsync");
        }

        [Fact]
        public void DeleteAccountNumberAsync_DeleteAccountNumberAsyncBuilderAccountNumberIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Builder() { AccountNumber = null });

            // Act
            var result = repository.DeleteAccountNumberAsync(0, "AccountNumber");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness Invalid Account number from DeleteAccountNumberAsync");
        }

        [Fact]
        public void DeleteAccountNumberAsync_DeleteAccountNumberAsyncBuilderAccountNumberIsNotEqualTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Builder() { AccountNumber = "DifAccountNumber" });

            // Act
            var result = repository.DeleteAccountNumberAsync(0, "AccountNumber");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness Invalid Account number from DeleteAccountNumberAsync");
        }

        [Fact]
        public void DeleteAccountNumberAsync_DeleteAccountNumberAsyncTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Builder() { AccountNumber = "AccountNumber" });

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.DeleteAccountNumberAsync(0, "AccountNumber");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : EndUserRefreshAsync
        [Fact]
        public void EndUserRefreshAsync_EndUserRefreshAsyncEndUserIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<EndUser>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(EndUser));

            // Act
            var result = repository.EndUserRefreshAsync(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void EndUserRefreshAsync_EndUserRefreshAsyncEndUserExistsTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<EndUser>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new EndUser());

            // Act
            var result = repository.EndUserRefreshAsync(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : IsValidBuilder
        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullAddress1_Test()
        {
            // Arrange
            var tesBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = "Postcode"

            };

            // Act
            var result = await repository.IsValidBuilder(tesBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = "Postcode"

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = null

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullAddress1AndTradingName_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = null,
                Postcode = "Postcode"

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullAddress1AndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = "TradingName",
                Postcode = null

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_NullTradingNameAndPostcode_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel()
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = null,
                Postcode = null

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberAndNullMandatoryFields_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = null,
                TradingName = null,
                Postcode = null

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(1, result.ErrorList.Count);
            Assert.Contains(result.ErrorList, e => e == "ArgumentErrorBusiness");
        }

        [Fact]
        public async void IsValidBuilder_IsValidBuilder_NullAccountNumber_NullAccountNumberNotNullMandatoryFields_Test()
        {
            // Arrange
            var testBuilderModel = new BuilderModel
            {
                AccountNumber = null,
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "Postcode"

            };

            // Act
            var result = await repository.IsValidBuilder(testBuilderModel);

            // Assert
            Assert.Equal(0, result.ErrorList.Count);
        }

        [Fact]
        public void IsValidBuilder_IsValidBuilder_NotNullAccountNumber_Test()
        {
            // Arrange

            // Act
            var result = repository.IsValidBuilder(new BuilderModel()
            {
                AccountNumber = "AccountNumber",
                Address1 = "Address1",
                TradingName = "TradingName",
                Postcode = "Postcode"

            });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result); ;
        }
        #endregion

        #region Func : BuilderCleanManagment
        [Fact]
        public void BuilderCleanManagment_BuilderCleanManagmentNullBuilderTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Builder));

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            // Act
            var result = repository.BuilderCleanManagment(0);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "ArgumentErrorBusiness Invalid Builder from BuilderCleanManagment");
        }

        [Fact]
        public void BuilderCleanManagment_BuilderCleanManagmentNotNullBuilderTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Builder>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Builder());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.BuilderCleanManagment(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponseGeneric>(result.Result);
        }
        #endregion

        #region Func : GetBuildersFiltered
        [Fact]
        public void GetBuildersFiltered_GetBuildersFilteredOver1000BuildersTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<BuilderMaterializedBuilderModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<BuilderModel>(new List<BuilderModel>(),
                    take: null,
                    skip: null,
                    total: 1001));

            // Act
            var result = repository.GetBuildersFiltered(searchModel, 0);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<BuilderModel>>>(result.Result);
            Assert.Contains(result.Result.ErrorList, e => e == "MaxTakeExceeded The maximum number of builders has been exceeded from GetBuildersFiltered");
        }

        [Fact]
        public void GetBuildersFiltered_GetBuildersFilteredUnder1000BuildersTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<BuilderMaterializedBuilderModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<BuilderModel>(new List<BuilderModel>(),
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetBuildersFiltered(searchModel, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IPagedQueryResult<BuilderModel>>>(result.Result);
        }
        #endregion

        #region Func : UpdateBuildersFromSapAsync
        [Fact]
        public void UpdateBuildersFromSapAsync_NullAccountNumber_ExistingBuilderIsNull_Test()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = null }
            });

            // Assert
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
        }

        [Fact]
        public void UpdateBuildersFromSapAsync_NotNullAccountNumber_ExistingBuilderIsNull_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = "AccountNumber" }
            });

            // Assert
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
        }

        [Fact]
        public void UpdateBuildersFromSapAsync_NotNullAccountNumber_ExistingBuilderIsNotNull_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockEntityRepository.Setup(er => er.GetAll<Plan>())
                .Returns(new List<Plan>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderSapModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = "AccountNumber" }
            });

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<BuilderModel>>>(result.Result);
        }

        [Fact]
        public void UpdateBuildersFromSapAsync_NotNullAccountNumber_ExistingBuilderNotFound_ExistingBuilderExists_NullAccountNumber_Test()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object)
                .Returns(new List<Builder>() { new Builder() { AccountNumber = null } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderSapModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = "AccountNumber" }
            });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
            Assert.IsType<RepositoryResponse<ICollection<BuilderModel>>>(result.Result);
        }

        [Fact]
        public void UpdateBuildersFromSapAsync_NotNullAccountNumber_ExistingBuilderNotFound_ExistingBuilderExists_NotNullAccountNumber_SameAsBuilderSapAccountNumber_Test()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
               .Returns(new List<Builder>().AsQueryable().BuildMock().Object)
               .Returns(new List<Builder>() { new Builder() { AccountNumber = "AccountNumber" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<BuilderSapModel>(), It.IsAny<Builder>()))
                .Returns(new Builder());

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = "AccountNumber" }
            });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
            Assert.IsType<RepositoryResponse<ICollection<BuilderModel>>>(result.Result);
        }

        [Fact]
        public void UpdateBuildersFromSapAsync_NotNullAccountNumber_ExistingBuilderNotFound_ExistingBuilderExists_NullAccountNumber_DifferentAsBuilderSapAccountNumber_Test()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
               .Returns(new List<Builder>().AsQueryable().BuildMock().Object)
               .Returns(new List<Builder>() { new Builder() { AccountNumber = "AccountNumberDif" } }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdateBuildersFromSapAsync(new List<BuilderSapModel>()
            {
                new BuilderSapModel() { AccountNumber = "AccountNumber" }
            });

            // Assert
            Assert.NotEmpty(result.Result.ErrorList);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
        }
        #endregion

        #region Func : UpdateBuilderFromSAPByAccountNumberAsync
        [Fact]
        public void UpdateBuilderFromSAPByAccountNumberAsync_UpdateBuilderFromSAPByAccountNumberAsync_TdpBuilderEntityIsNotNull_TdpBuilderEntityAccountNumberIsNull_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<Builder>(), It.IsAny<BuilderModel>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuilderFromSAPByAccountNumberAsync(new BuilderModel(), new BuilderModel());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }

        [Fact]
        public void UpdateBuilderFromSAPByAccountNumberAsync_UpdateBuilderFromSAPByAccountNumberAsync_TdpBuilderEntityIsNotNull_TdpBuilderEntityAccountNumberIsNotNull_DifferentFromSAP_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() { AccountNumber = "AccountNumber" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<Builder>(), It.IsAny<BuilderModel>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuilderFromSAPByAccountNumberAsync(new BuilderModel() { AccountNumber = "AccountNumber" }, new BuilderModel());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }

        [Fact]
        public void UpdateBuilderFromSAPByAccountNumberAsync_UpdateBuilderFromSAPByAccountNumberAsync_TdpBuilderEntityIsNotNull_TdpBuilderEntityAccountNumberIsNotNull_SameAsSAP_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() { AccountNumber = "AccountNumber" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<Builder>(), It.IsAny<BuilderModel>()))
                .Returns(new BuilderModel());

            // Act
            var result = repository.UpdateBuilderFromSAPByAccountNumberAsync(new BuilderModel() { AccountNumber = "" }, new BuilderModel());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void UpdateBuilderFromSAPByAccountNumberAsync_UpdateBuilderFromSAPByAccountNumberAsync_TdpBuilderEntityIsNotNull_OtherBuilderIsNull_Test()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.UpdateBuilderFromSAPByAccountNumberAsync(new BuilderModel(), new BuilderModel());

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
        }

        [Fact]
        public void UpdateBuilderFromSAPByAccountNumberAsync_UpdateBuilderFromSAPByAccountNumberAsync_TdpBuilderEntityIsNotNull_OtherBuilderIsNotNull_Test()
        {
            // Arrange
            mockEntityRepository.SetupSequence(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object)
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map(It.IsAny<Builder>(), It.IsAny<BuilderModel>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateBuilderFromSAPByAccountNumberAsync(new BuilderModel(), new BuilderModel());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : GetBuildersOmniSearch
        [Fact]
        public void GetBuildersOmniSearch_GetBuildersOmniSearchNullOrEmptyTextToSearchTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder> { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetBuildersOmniSearch("", 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>>(result.Result);
        }

        [Fact]
        public void GetBuildersOmniSearch_GetBuildersOmniSearchNotNullOrEmptyTextToSearchTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.GetAll<Builder>())
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<IEnumerable<Builder>>()))
                .Returns(new List<BuilderModel>() { new BuilderModel() });

            // Act
            var result = repository.GetBuildersOmniSearch("textToSearch", 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>>(result.Result);
        }
        #endregion

        #region Func : GetAssignedPlansAsync
        [Fact]
        public void GetAssignedPlansAsync_GetAssignedPlansAsyncNullBuilderTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetAssignedPlansAsync(1);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }

        [Fact]
        public void GetAssignedPlansAsync_GetAssignedPlansAsyncNotNullBuilderTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetAssignedPlansAsync(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<Builder>>(result.Result);
        }
        #endregion

        #region Func : MergeBuilderTdpAndSapSearch
        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderModelAndBuildersNotNull_SapValidationBuilderModelAndBuildersNotNull_AddItem_Test()
        {
            // Arrange
            var tdpValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            var sapValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber2" }
                    }
                }
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(tdpValidationBuilderModel, sapValidationBuilderModel);

            // Assert
            Assert.Equal(2, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderModelAndBuildersNotNull_SapValidationBuilderModelAndBuildersNotNull_SameBuilder_Test()
        {
            // Arrange
            var tdpValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            var sapValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(tdpValidationBuilderModel, sapValidationBuilderModel);

            // Assert
            Assert.Equal(1, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderIsNull_SapValidationBuilderModelAndBuildersNotNull_SameBuilder_Test()
        {
            // Arrange
            var sapValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(null, sapValidationBuilderModel);

            // Assert
            Assert.Equal(1, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderBuildersAreNull_SapValidationBuilderModelAndBuildersNotNull_SameBuilder_Test()
        {
            // Arrange
            var tdpValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = null
            };

            var sapValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(tdpValidationBuilderModel, sapValidationBuilderModel);

            // Assert
            Assert.Equal(1, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderAndBuildersNotNull_SapValidationBuilderModelIsNull_Test()
        {
            // Arrange
            var tdpValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(tdpValidationBuilderModel, null);

            // Assert
            Assert.Equal(1, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderAndBuildersNotNull_SapValidationBuilderModelBuildersAreNull_Test()
        {
            // Arrange
            var tdpValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = new List<BuilderSapSearch>()
                {
                    new BuilderSapSearch()
                    {
                        Builder = new BuilderModel() { AccountNumber = "AccountNumber1" }
                    }
                }
            };

            var sapValidationBuilderModel = new ValidationBuilderModel()
            {
                Builders = null
            };

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(tdpValidationBuilderModel, sapValidationBuilderModel);

            // Assert
            Assert.Equal(1, result.Content.Builders.Count);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }

        [Fact]
        public void MergeBuilderTdpAndSapSearch_MergeBuilderTdpAndSapSearchTdpValidationBuilderIsNull_SapValidationBuilderModelIsNull_Test()
        {
            // Arrange

            // Act
            var result = repository.MergeBuilderTdpAndSapSearch(null, null);

            // Assert
            Assert.Empty(result.Content.Builders);
            Assert.IsType<ValidationBuilderModel>(result.Content);
        }
        #endregion

        #region Func : ModifyBuilderNotes
        [Fact]
        public void ModifyBuilderNotes_ModifyBuilderNotesBuilderIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ModifyBuilderNotes(0, "notes");

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound Builder not found from ModifyBuilderNotes");
        }

        [Fact]
        public void ModifyBuilderNotes_ModifyBuilderNotesBuilderExistsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Builder>>()))
                .Returns(new List<Builder>() { new Builder() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Builder, BuilderModel>(It.IsAny<Builder>()))
                .Returns(new BuilderModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.ModifyBuilderNotes(1, "notes");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<BuilderModel>>(result.Result);
        }
        #endregion

        #region Func : CallIndexerAsync
        
        #endregion
    }
}


