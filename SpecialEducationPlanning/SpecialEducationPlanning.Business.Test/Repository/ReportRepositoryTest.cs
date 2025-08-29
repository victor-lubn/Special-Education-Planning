using System;
using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification.Search;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Report;
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
    public class ReportRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly ReportRepository repository;

        public ReportRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new ReportRepository(
                this.LoggerFactory.CreateLogger<ReportRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Funf : GetReport
        [Fact]
        public void GetReportTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.GetAll<Aiep>())
                .Returns(new List<Aiep>()
                {
                    new Aiep()
                    {
                        Projects = new List<Project>()
                        {
                            new Project()
                            {
                                Plans = new List<Plan>()
                                {
                                    new Plan { UpdatedDate = DateTime.Today },
                                    new Plan { UpdatedDate = DateTime.MinValue}
                                }
                            }
                        }
                    }
                }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.GetReport(DateTime.MinValue, DateTime.Now);

            // Assert
            Assert.IsType<RepositoryResponse<ICollection<AiepReportModel>>>(result.Result);
        }
        #endregion
    }
}

