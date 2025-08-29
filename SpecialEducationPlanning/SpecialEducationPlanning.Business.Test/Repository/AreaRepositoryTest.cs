using System.Linq;
using System.Collections.Generic;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using System;
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
.Business.DtoModel;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using Xunit;
using MockQueryable.Moq;
using Xunit.Abstractions;
using Koa.Domain.Search.Page;
using System.Threading;
using SpecialEducationPlanning
.Domain.Specification.AreaSpecifications;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class AreaRepositoryTest : BaseTest
    {
        private readonly int existingAreaId = 1;
        private readonly int nonExistingAreaId = 99;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly AreaRepository repository;

        public AreaRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new AreaRepository(
                this.LoggerFactory.CreateLogger<AreaRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        private AreaDtoModel ExistingAreaDtoModel
        {
            get
            {
                return new AreaDtoModel()
                {
                    Id = 1,
                    KeyName = "KeyName",
                    RegionId = 1
                };
            }
        }

        private AreaDtoModel NonExistingAreaDtoModel
        {
            get
            {
                return new AreaDtoModel()
                {
                    Id = 0,
                    KeyName = "KeyName",
                    RegionId = 1
                };
            }
        }

        private AreaDtoModel NewExistingAreaDtoModel
        {
            get
            {
                return new AreaDtoModel()
                {
                    Id = 1,
                    KeyName = "KeyName",
                    RegionId = 1
                };
            }
        }


        private Area ExistingArea
        {
            get
            {
                return new Area()
                {
                    Id = 0,
                    KeyName = "KeyName",
                    RegionId = 1
                };
            }
        }

        #region Func: GetAreaAieps
        [Fact]
        public void GetAreaAieps_GetAllAiepsInExistingAreaTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>() { new Area { Id = existingAreaId } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Area, AreaModel>(It.IsAny<Area>()).Aieps)
                .Returns(new List<AiepModel>() { new AiepModel() });

            // Act
            var result = repository.GetAreaAieps(existingAreaId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<ICollection<AiepModel>>>(result.Result);
        }

        [Fact]
        public void GetAreaAieps_GetAllAiepsInNullAreaTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>().AsQueryable().BuildMock().Object);

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

        #region Func: SaveArea

        [Fact]
        public void SaveArea_SaveNewAreaExistingBySpecificationsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>() { new Area { Id = existingAreaId, KeyName = "KeyName" } }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<Area, AreaModel>(It.IsAny<Area>()))
                .Returns(new AreaModel());

            // Act
            var result = repository.SaveArea(NonExistingAreaDtoModel);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e.Contains(ErrorCode.EntityAlreadyExist.GetDescription() + " Area already exists"));
        }

        [Fact]
        public void SaveArea_SaveNewAreaNonExistingTest()
        {
            // Arrange

            var newArea = new Area { Id = 2, KeyName = "NewArea" };

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Area>>()))
                .Returns(new List<Area>().AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Add(It.IsAny<Area>()))
                .Returns(new Area());

            mockObjectMapper.Setup(m => m.Map<AreaDtoModel, Area>(It.IsAny<AreaDtoModel>()))
                .Returns(new Area());


            mockObjectMapper.Setup(m => m.Map(It.IsAny<AreaDtoModel>(), It.IsAny<Area>()))
              .Returns(newArea);

     

            var mockQueryable = new List<Area> { newArea }.AsQueryable().BuildMock();


            mockEntityRepository.Setup(r => r.Where(It.IsAny<AreaByNameSpecification>()))
                .Returns(mockQueryable.Object);

            mockEntityRepositoryKey.Setup(rep => rep.FindOneAsync<Area>(It.IsAny<int>(), System.Threading.CancellationToken.None))
                    .ReturnsAsync(new Area());
            mockObjectMapper.Setup(m => m.Map<Area, AreaModel>(It.IsAny<Area>()))
                .Returns(new AreaModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.SaveArea(NewExistingAreaDtoModel);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<AreaModel>>(result.Result);
        }

        #endregion
    }
}

