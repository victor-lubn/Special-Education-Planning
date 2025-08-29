using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.CommentSpecifications;

using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly IEfUnitOfWork unitOfWork;
        private readonly IObjectMapper mapper;
        private readonly IDbExecutionStrategy executionStrategy;

        private readonly ILogger<CommentRepository> logger;

        public CommentRepository(ILogger<CommentRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IDbExecutionStrategy executionStrategy, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.executionStrategy = executionStrategy;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<IEnumerable<CommentModel>>> GetModelComments<T>(int id)
        {
            logger.LogDebug("CommentRepository called GetModelComments");

            ISpecification<Comment> spec = new Specification<Comment>(x => x.EntityName == GetEntityName<T>());
            spec = spec.And(new Specification<Comment>(x => x.EntityId == id));

            var result = await repository.Where(spec).OrderByDescending(x => x.CreatedDate).ToListAsync();

            logger.LogDebug("CommentRepository end call GetModelComments -> return Repository response List of Comment model");

            return new RepositoryResponse<IEnumerable<CommentModel>>(mapper.Map<IEnumerable<Comment>, IEnumerable<CommentModel>>(result));
        }

        public async Task<RepositoryResponse<CommentModel>> CreateComment<T>(CommentModel commentModel, int modelId, string userInfo)
        {
            logger.LogDebug("CommentRepository called CreateComment");
            var commentEntity = mapper.Map(commentModel, new Comment());

            this.executionStrategy.Execute(() =>
            {
                unitOfWork.BeginTransaction();

                commentEntity.EntityId = modelId;
                commentEntity.EntityName = GetEntityName<T>();
                commentEntity.User = userInfo;

                repository.Add(commentEntity);

                logger.LogDebug("CommentRepository CreateComment Commit");

                unitOfWork.Commit();
            });

            logger.LogDebug("CommentRepository end call CreateComment -> return Repository response Comment model");

            return new RepositoryResponse<CommentModel>
            {
                Content = mapper.Map<Comment, CommentModel>(commentEntity)
            };
        }

        public async Task<RepositoryResponse<CommentModel>> UpdateComment<T>(CommentModel commentModel, string userUniqueIdentifier)
        {
            logger.LogDebug("CommentRepository called UpdateComment");

            var response = new RepositoryResponse<CommentModel>();
            var comment = await repository.Where(new CommentByIdSpecification(commentModel.Id)).FirstOrDefaultAsync();

            if (comment == null)
            {
                response.ErrorList.Add(ErrorCode.NoResults.GetDescription());

                logger.LogDebug("CommentRepository end call UpdateComment -> return Repository response Errors No result");

                return response;
            }

            return this.UpdateCommentExecStrategy<Plan>(commentModel, userUniqueIdentifier, comment);
        }

        public RepositoryResponse<CommentModel> UpdateCommentExecStrategy<T>(CommentModel commentModel, string userUniqueIdentifier, Comment comment)
        {
            RepositoryResponse<CommentModel> response = new();

            this.executionStrategy.Execute(() =>
            {
                unitOfWork.BeginTransaction();

                mapper.Map<CommentModel, Comment>(commentModel, comment);
                comment.EntityName = GetEntityName<T>();
                comment.User = userUniqueIdentifier;
                response.Content = mapper.Map<Comment, CommentModel>(comment);
                unitOfWork.Commit();

                logger.LogDebug("CommentRepository UpdateComment Commit");
                unitOfWork.Commit();

            });

            logger.LogDebug("CommentRepository end call UpdateComment -> return Repository response Comment model");
            return response;
        }

        #region Private Method
        private string GetEntityName<T>()
        {
            logger.LogDebug("CommentRepository called GetEntityName");

            var entityName = typeof(T).Name;
            if (entityName.Contains("Model"))
            {
                entityName = entityName.Substring(0, entityName.Length - 5);
            }

            logger.LogDebug("CommentRepository end call GetEntityName -> return String entity name");

            return entityName;
        }
        #endregion
    }
}
