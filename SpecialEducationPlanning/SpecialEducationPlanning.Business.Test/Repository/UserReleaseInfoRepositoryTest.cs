using System.Linq;
using System.Collections.Generic;

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
using SpecialEducationPlanning
.Business.Repository.UserReleaseInfoRepository;

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
    public class UserReleaseInfoRepositoryTest : BaseTest
    {
        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;
        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;
        private readonly Mock<IReleaseInfoRepository> mockReleaseInfoRepository;

        private readonly UserReleaseInfoRepository repository;

        public UserReleaseInfoRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);
            mockReleaseInfoRepository = new Mock<IReleaseInfoRepository>(MockBehavior.Strict);

            repository = new UserReleaseInfoRepository(
                LoggerFactory.CreateLogger<UserReleaseInfoRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                mockReleaseInfoRepository.Object,
                new SpecificationBuilder(LoggerFactory.CreateLogger<SpecificationBuilder>()),
                mockEntityRepository.Object
            );
        }

        #region Func : ExistsUserReleaseInfo
        [Fact]
        public void ExistsUserReleaseInfo_ExistsUserReleaseInfoDoesNotExists()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserReleaseInfo>>()))
                .Returns(new List<UserReleaseInfo>().AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ExistsUserReleaseInfo(0, 0);

            // Assert
            Assert.False(result.Result);
        }

        [Fact]
        public void ExistsUserReleaseInfo_ExistsUserReleaseInfoExists()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserReleaseInfo>>()))
                .Returns(new List<UserReleaseInfo>() { new UserReleaseInfo() }.AsQueryable().BuildMock().Object);

            // Act
            var result = repository.ExistsUserReleaseInfo(0, 0);

            // Assert
            Assert.True(result.Result);
        }
        #endregion

        #region Func : CreateUserReleaseInfoAsync
        [Fact]
        public void CreateUserReleaseInfoAsyncTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Add(It.IsAny<UserReleaseInfo>()))
                .Returns(new UserReleaseInfo());

            mockObjectMapper.Setup(m => m.Map<UserReleaseInfo, UserReleaseInfoModel>(It.IsAny<UserReleaseInfo>()))
                .Returns(new UserReleaseInfoModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateUserReleaseInfoAsync(0, 0);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<UserReleaseInfoModel>>(result.Result);
        }
        #endregion

        #region Func : DeleteUserReleaseInfoAsync
        [Fact]
        public void DeleteUserReleaseInfoAsync_DeleteUserReleaseInfoAsyncReleaseInfoIsNotNullTest()
        {
            // Arrange
            mockReleaseInfoRepository.Setup(rr => rr.GetReleaseInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResponse<ReleaseInfoModel>()
                {
                    Content = new ReleaseInfoModel()
                    {
                        Id = 1
                    }
                });

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserReleaseInfo>>()))
                .Returns(new List<UserReleaseInfo>() { new UserReleaseInfo() { Id = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Remove<UserReleaseInfo>(It.IsAny<int>())).Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.DeleteUserReleaseInfoAsync("version", "fusionVersion");

            // Assert
            Assert.True(result.Result);
        }

        [Fact]
        public async void DeleteUserReleaseInfoAsync_DeleteUserReleaseInfoAsyncReleaseInfoIsNullTest()
        {
            //Arrange
            mockReleaseInfoRepository.Setup(rr => rr.GetReleaseInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync((RepositoryResponse<ReleaseInfoModel>)null);

            mockReleaseInfoRepository.Setup(rr => rr.GetReleaseInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResponse<ReleaseInfoModel>()
                {
                    Content = new ReleaseInfoModel()
                    {
                        Id = 1
                    }
                });

            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<UserReleaseInfo>>()))
                .Returns(new List<UserReleaseInfo>() { new UserReleaseInfo() { Id = 1 } }.AsQueryable().BuildMock().Object);

            mockEntityRepositoryKey.Setup(er => er.Remove<UserReleaseInfo>(It.IsAny<int>())).Verifiable();

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = await repository.DeleteUserReleaseInfoAsync("version", "fusionVersion");

            // Assert
            Assert.True(result);
        }
        #endregion
    }
}
