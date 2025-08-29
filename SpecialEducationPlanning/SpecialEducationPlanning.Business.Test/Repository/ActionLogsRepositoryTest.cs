using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.QueryResult;

using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Query;

using Moq;
using Xunit;
using MockQueryable.Moq;
using System;
using SpecialEducationPlanning
.Business.Model.View;
using SpecialEducationPlanning
.Domain.Entity.View;
using System.Text;
using SpecialEducationPlanning
.Business.Test;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Koa.Domain.Specification.Search;
using Koa.Domain.Search.Page;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class ActionLogsRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;

        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly ActionLogsRepository repository;

        public ActionLogsRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>();
            mockEntityRepository = new Mock<IEntityRepository>();

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);

            //Currently in default behavior
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Default);

            repository = new ActionLogsRepository(
                this.LoggerFactory.CreateLogger<ActionLogsRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetActionLogsFilteredAsync
        [Fact]
        public void GetActionLogsFilteredAsyncTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepositoryKey.Setup(er => er.Query(It.IsAny<ActionLogsMaterializedActionLogsModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<ActionLogsModel>(new List<ActionLogsModel>()
                    {
                        new ActionLogsModel()
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetActionLogsFilteredAsync(searchModel);

            // Assert
            Assert.IsType<RepositoryResponse<IPagedQueryResult<ActionLogsModel>>>(result.Result);
        }
        #endregion

        #region Func : GetActionLogsCsv
        [Fact]
        public void GetActionLogsCsv_GetActionLogsCsvCountIsZeroTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<ActionLogs>>()))
                .Returns(new List<ActionLogs>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetActionLogsCsv(new DateTime(), new DateTime());

            // Assert
            Assert.IsType<RepositoryResponse<StringBuilder>>(result.Result);
        }

        [Fact]
        public void GetActionLogsCsv_GetActionLogsCsvCountIsNotZeroTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<ActionLogs>>()))
                .Returns(new List<ActionLogs>() { new ActionLogs() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetActionLogsCsv(new DateTime(), new DateTime());

            // Assert
            Assert.IsType<RepositoryResponse<StringBuilder>>(result.Result);
        }
        #endregion
    }
}
