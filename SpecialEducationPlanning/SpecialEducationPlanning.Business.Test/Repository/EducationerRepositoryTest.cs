using System.Threading;


using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Domain.Specification.Search;

using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Test;

using Moq;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    [Trait("Repository", "")]
    [Trait("Unit", "")]
    public class EducationerRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly EducationerRepository repository;

        public EducationerRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);

            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new EducationerRepository(
                this.LoggerFactory.CreateLogger<EducationerRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : GetPendingReleaseInfo
        [Fact]
        public void GetPendingReleaseInfo_GetPendingReleaseInfoEducationerAiepIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User() { AiepId = null });

            // Act
            var result = repository.GetPendingReleaseInfo(1);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound  from GetPendingReleaseInfo");
        }

        [Fact]
        public void GetPendingReleaseInfo_GetPendingReleaseInfoAiepIsNullTest()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User() { AiepId = 1 });

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(default(Aiep));

            // Act
            var result = repository.GetPendingReleaseInfo(1);

            // Assert
            Assert.Null(result.Result.Content);
            Assert.Contains(result.Result.ErrorList, e => e == "EntityNotFound AiepId not found from GetPendingReleaseInfo");
        }

        [Fact]
        public void GetPendingReleaseInfo_GetPendingReleaseInfoEducationerAiepExists()
        {
            // Arrange
            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<User>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new User() { AiepId = 1 });

            mockEntityRepositoryKey.Setup(er => er.FindOneAsync<Aiep>(It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(new Aiep());

            // Act
            var result = repository.GetPendingReleaseInfo(1);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<int?>>(result.Result);
        }
        #endregion
    }
}


