using System;
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
    public class LogRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly LogRepository repository;

        public LogRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            //Behaviour is Default for now
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Default);

            repository = new LogRepository(
                this.LoggerFactory.CreateLogger<LogRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetAllLog
        [Fact]
        public void GetAllLogTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<Log>())
                .Returns(new List<Log>() { new Log() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Log, LogModel>(It.IsAny<IEnumerable<Log>>()))
                .Returns(new List<LogModel>());

            // Act
            var result = repository.GetAllLog();

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<LogModel>>>(result.Result);
        }
        #endregion

        #region Func : GetLogsFiltered
        [Fact]
        public void GetLogsFilteredTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.Query(It.IsAny<LogMaterializedLogModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<LogModel>(new List<LogModel>()
                    {
                        new LogModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetLogsFiltered(new PageDescriptor(null, null));

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IPagedQueryResult<LogModel>>>(result.Result);
        }
        #endregion

        #region Func : GetLogsFilteredAsync
        [Fact]
        public void GetLogsFilteredAsync_GetLogsFilteredAsyncInitDateOrEndDateOrLevelHasNoValueTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Log>>()))
                .Returns(new List<Log>() { new Log() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Log>, IEnumerable<LogModel>>(It.IsAny<IEnumerable<Log>>()))
                .Returns(new List<LogModel>());

            // Act
            var result = repository.GetLogsFilteredAsync("", null, null);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<LogModel>>>(result.Result);
        }

        [Fact]
        public void GetLogsFilteredAsync_GetLogsFilteredAsyncInitDateAndEndDateHaveValueNotLevelTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Log>>()))
                .Returns(new List<Log>() { new Log() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Log>, IEnumerable<LogModel>>(It.IsAny<IEnumerable<Log>>()))
                .Returns(new List<LogModel>());

            // Act
            var result = repository.GetLogsFilteredAsync("", new DateTime(), new DateTime());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<LogModel>>>(result.Result);
        }

        [Fact]
        public void GetLogsFilteredAsync_GetLogsFilteredAsyncInitDateAndEndDateHaveNoValueLevelHasValueTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Log>>()))
                .Returns(new List<Log>() { new Log() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Log>, IEnumerable<LogModel>>(It.IsAny<IEnumerable<Log>>()))
                .Returns(new List<LogModel>());

            // Act
            var result = repository.GetLogsFilteredAsync("level", null, null);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<LogModel>>>(result.Result);
        }

        [Fact]
        public void GetLogsFilteredAsync_GetLogsFilteredAsyncInitDateAndEndDateHaveValueLevelHasValueTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Log>>()))
                .Returns(new List<Log>() { new Log() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Log>, IEnumerable<LogModel>>(It.IsAny<IEnumerable<Log>>()))
                .Returns(new List<LogModel>());

            // Act
            var result = repository.GetLogsFilteredAsync("level", new DateTime(), new DateTime());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<LogModel>>>(result.Result);
        }
        #endregion

        #region Func : AutomaticRemoveOldItems

        #endregion

        #region Func : SaveExternalLog
        [Fact]
        public void SaveExternalLogTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Add(It.IsAny<Log>()))
                .Returns(new Log());

            mockObjectMapper.Setup(m => m.Map<LogModel, Log>(It.IsAny<LogModel>()))
                .Returns(new Log());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SaveExternalLog(new LogModel());

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<Log>>(result.Result);
        }
        #endregion
    }
}
