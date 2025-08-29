using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Query;

using Moq;
using Xunit;
//using MockQueryable.Moq;
using SpecialEducationPlanning
.Business.Test;
using Xunit.Abstractions;
using MockQueryable.Moq;
using Koa.Domain.Search.Page;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class AreaRepositoryTestt : BaseTest
    {
        private readonly int existingAreaId = 1;
        private readonly int nonExistingAreaId = 99;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;

        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;


        private readonly AreaRepository repository;
        private ILogger<AreaRepositoryTest> logger;
        private AutoMapperObjectMapper koaMapper;

        public AreaRepositoryTestt(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            this.logger = this.LoggerFactory.CreateLogger<AreaRepositoryTest>();
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AreaModelProfile>();
            });
            this.koaMapper = new AutoMapperObjectMapper(config.CreateMapper());

            repository = new AreaRepository(
                this.LoggerFactory.CreateLogger<AreaRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                this.koaMapper,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func: GetAreaAieps
        [Fact]
        public void GetAllAiepsInExistingAreaTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>() { new Area { Id = existingAreaId } }.AsQueryable()
                    .BuildMock().Object
                    );

            // Act
            var result = repository.GetAreaAieps(existingAreaId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<AiepModel>>>(result.Result);
        }

        [Fact]
        public void GetAllAiepsInNullAreaTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>().AsQueryable()
                    .BuildMock().Object
                );

            // Act
            var result = repository.GetAreaAieps(existingAreaId);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound");
        }
        #endregion

        #region Func: GetAreasFiltered
        [Fact]
        public void GetAreasFilteredTest()
        {
            // Arrange
            PageDescriptor searchModel = new PageDescriptor(null, null);

            mockEntityRepository.Setup(er => er.Query(It.IsAny<AreaMaterializedAreaModelPagedValueQuery>()))
                .Returns(new PagedQueryResult<AreaModel>(new List<AreaModel>()
                    {
                        new AreaModel()
                        {
                            Id = existingAreaId
                        }
                    },
                    take: null,
                    skip: null,
                    total: 1));

            // Act
            var result = repository.GetAreasFilteredAsync(searchModel);

            // Arrange
            Assert.IsType<RepositoryResponse<IPagedQueryResult<AreaModel>>>(result.Result);

        }
        #endregion
    }
}

