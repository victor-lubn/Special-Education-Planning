using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;

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

using Moq;
using MockQueryable.Moq;
using Xunit;
using Xunit.Abstractions;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.UnitTest.Repository
{
    public class CommentRepositoryTest : BaseTest
    {
        private readonly int existingId = 1;

        private readonly Mock<IEntityRepository<int>> mockEntityRepositoryKey;
        private readonly Mock<IEntityRepository> mockEntityRepository;

        private readonly Mock<IEfUnitOfWork> mockUnitOfWork;
        private readonly Mock<IObjectMapper> mockObjectMapper;
        private readonly Mock<IDbContextAccessor> mockDbContextAccessor;

        private readonly CommentRepository repository;

        public CommentRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            mockEntityRepositoryKey = new Mock<IEntityRepository<int>>(MockBehavior.Strict);
            mockEntityRepository = new Mock<IEntityRepository>(MockBehavior.Strict);
            mockUnitOfWork = new Mock<IEfUnitOfWork>(MockBehavior.Strict);
            mockObjectMapper = new Mock<IObjectMapper>(MockBehavior.Strict);
            mockDbContextAccessor = new Mock<IDbContextAccessor>(MockBehavior.Strict);

            repository = new CommentRepository(
                this.LoggerFactory.CreateLogger<CommentRepository>(),
                mockEntityRepositoryKey.Object,
                mockUnitOfWork.Object,
                mockObjectMapper.Object,
                mockDbContextAccessor.Object,
                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>()),
                new SqlAzureExecutionStrategy(),
                mockEntityRepository.Object
            );
        }

        [Fact]
        #region Func : GetModelComments
        public void GetModelCommentsTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Comment>>()))
                .Returns(new List<Comment>() { new Comment() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<IEnumerable<Comment>, IEnumerable<CommentModel>>(It.IsAny<IEnumerable<Comment>>()))
                .Returns(new List<CommentModel>());

            // Act
            var result = repository.GetModelComments<int>(existingId);

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<IEnumerable<CommentModel>>>(result.Result);
        }
        #endregion

        #region Func : CreateComment
        [Fact]
        public void CreateCommentTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Add(It.IsAny<Comment>()))
                .Returns(new Comment());

            mockObjectMapper.Setup(m => m.Map<CommentModel, Comment>(It.IsAny<CommentModel>(), It.IsAny<Comment>()))
                .Returns(new Comment());

            mockObjectMapper.Setup(m => m.Map<Comment, CommentModel>(It.IsAny<Comment>()))
                .Returns(new CommentModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.CreateComment<int>(new CommentModel(), existingId, "userInfo");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<CommentModel>>(result.Result);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Result.Content);
        }

        #endregion

        #region Func : UpdateComment
        [Fact]
        public void UpdateComment_UpdateCommentCommentIsNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Comment>>()))
                .Returns(new List<Comment>().AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<CommentModel, Comment>(It.IsAny<CommentModel>(), It.IsAny<Comment>()))
                .Returns(new Comment());

            mockObjectMapper.Setup(m => m.Map<Comment, CommentModel>(It.IsAny<Comment>()))
                .Returns(new CommentModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateComment<Plan>(new CommentModel(), "userUniqueIdentifier");

            // Assert
            Assert.Contains(result.Result.ErrorList, e => e == "NoResults");
            Assert.IsType<RepositoryResponse<CommentModel>>(result.Result);
        }

        [Fact]
        public void UpdateComment_UpdateCommentCommentIsNotNullTest()
        {
            // Arrange
            mockEntityRepository.Setup(er => er.Where(It.IsAny<ISpecification<Comment>>()))
                .Returns(new List<Comment>() { new Comment() }.AsQueryable().BuildMock().Object);

            mockObjectMapper.Setup(m => m.Map<CommentModel, Comment>(It.IsAny<CommentModel>(), It.IsAny<Comment>()))
                .Returns(new Comment());

            mockObjectMapper.Setup(m => m.Map<Comment, CommentModel>(It.IsAny<Comment>()))
                .Returns(new CommentModel());

            mockUnitOfWork.Setup(un => un.BeginTransaction()).Verifiable();

            mockUnitOfWork.Setup(un => un.Commit()).Verifiable();

            // Act
            var result = repository.UpdateComment<Plan>(new CommentModel(), "userUniqueIdentifier");

            // Assert
            Assert.Empty(result.Result.ErrorList);
            Assert.IsType<RepositoryResponse<CommentModel>>(result.Result);
        }

        #endregion
    }
}
