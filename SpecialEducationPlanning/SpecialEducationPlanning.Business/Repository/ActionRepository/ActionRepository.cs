using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;
using Action = SpecialEducationPlanning
.Domain.Entity.Action;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class ActionRepository : BaseRepository<Action>, IActionRepository
    {
        private readonly IEfUnitOfWork unitOfWork;
        private readonly IObjectMapper mapper;
        private readonly ILogger<ActionRepository> logger;

        public ActionRepository(ILogger<ActionRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
                        base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)

        {

            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<ICollection<ActionModel>>> GetModelActions<T>(int id)
        {
            logger.LogDebug("ActionRepository called GetModelActions");

            ISpecification<Action> spec = new Specification<Action>(x => x.EntityName == GetEntityName<T>());
            spec = spec.And(new Specification<Action>(x => x.EntityId == id));

            var result = await repository.Where(spec).OrderByDescending(x => x.Date).ToListAsync();

            logger.LogDebug("ActionRepository end call GetModelActions -> return Repository response Collection of ActionModel");

            return new RepositoryResponse<ICollection<ActionModel>>(mapper.Map<ICollection<Action>, ICollection<ActionModel>>(result));
        }

        public async Task<RepositoryResponse<ICollection<ActionModel>>> GetPlanActions<TPlan, TVersion>(int id)
        {
            logger.LogDebug("ActionRepository called GetPlanActions");

            var versions = await base.Repository.GetVersionsByPlanIdAsync(id);

            ISpecification<Action> planSpec = new Specification<Action>(x => x.EntityName == GetEntityName<TPlan>());
            planSpec = planSpec.And(new Specification<Action>(x => x.EntityId == id));

            ISpecification<Action> versionNameSpec = new Specification<Action>(x => x.EntityName == GetEntityName<TVersion>());

            ISpecification<Action> versionSpec = new Specification<Action>(x => x.EntityId == 0);

            foreach (var version in versions)
            {
                versionSpec = versionSpec.Or(new Specification<Action>(x => x.EntityId == version.Id));
            }

            var result = await repository.Where(planSpec.Or(versionNameSpec.And(versionSpec))).OrderByDescending(x => x.Date).ToListAsync();

            logger.LogDebug("ActionRepository end call GetPlansActions -> return Repository response Collection of ActionModel");

            return new RepositoryResponse<ICollection<ActionModel>>(mapper.Map<ICollection<Action>, ICollection<ActionModel>>(result));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionTypeEnum"></param>
        /// <param name="additionalInfo"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task CreateAction<T>(ActionType actionTypeEnum, string additionalInfo, int modelId, string userInfo)
        {
            logger.LogDebug("ActionRepository called CreateAction");

            // Save the action to track

            var actionModel = new ActionModel()
            {
                ActionType = actionTypeEnum,
                AdditionalInfo = additionalInfo,
                Date = DateTime.UtcNow,
                EntityId = modelId,
                EntityName = GetEntityName<T>(),
                User = userInfo
            };
            var actionEntity = mapper.Map<ActionModel, Action>(actionModel);
            await this.Add(actionEntity);

            logger.LogDebug("ActionRepository end call CreateAction");
        }


        private string GetEntityName<T>()
        {
            logger.LogDebug("ActionRepository called GetEntityName");

            var entityName = typeof(T).Name;
            if (entityName.Contains("Model"))
            {
                entityName = entityName.Substring(0, entityName.Length - 5);
            }

            logger.LogDebug("ActionRepository end call GetEntityName -> return Entity name");

            return entityName;
        }
    }
}
