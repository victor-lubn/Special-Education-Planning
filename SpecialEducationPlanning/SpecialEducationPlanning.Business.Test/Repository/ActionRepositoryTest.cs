using System.Linq;
using System.Collections.Generic;


using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification;

using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using Action = SpecialEducationPlanning
.Domain.Entity.Action;

using Moq;
using Xunit;
using MockQueryable.Moq;
using SpecialEducationPlanning
.Business.Test;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Koa.Domain.Specification.Search;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    public class ActionRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly ActionRepository repository;

        public ActionRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new ActionRepository(
                this.LoggerFactory.CreateLogger<ActionRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetModelActions
        [Fact]
        public void GetModelActionsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Action>>()))
                .Returns(new List<Action>() { new Action() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<ICollection<Action>, ICollection<ActionModel>>(It.IsAny<ICollection<Action>>()))
                .Returns(new List<ActionModel>());

            // Act
            var result = repository.GetModelActions<Action>(1);

            // Assert
            Assert.IsType<RepositoryResponse<ICollection<ActionModel>>>(result.Result);
        }
        #endregion
    }
}
