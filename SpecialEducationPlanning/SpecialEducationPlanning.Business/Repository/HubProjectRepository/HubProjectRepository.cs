using Koa.Domain.Specification.Search;
using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class HubProjectRepository : BaseRepository<Project>, IHubProjectRepository
    {
        private readonly IObjectMapper mapper;
        private readonly IEntityRepository entityRepositoryKey;
        private readonly ILogger<ProjectRepository> logger;

        public HubProjectRepository(ILogger<ProjectRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) :
            base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RepositoryResponse<ProjectModel>> CreateProjectForPlan(PlanModel value, int AiepId)
        {
            logger.LogDebug("ProjectRepository called CreateProjectForPlan");

            if (AiepId == 0)
            {
                logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response Errors UndefinedAiep");

                return new RepositoryResponse<ProjectModel>(null, ErrorCode.UndefinedAiep);
            }

            if (!repository.Any(new EntityByIdSpecification<Aiep>(AiepId)))
            {
                logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response Errors Argument error business");

                return new RepositoryResponse<ProjectModel>(null, ErrorCode.ArgumentErrorBusiness);
            }

            var newProject = new ProjectModel
            {
                AiepId = AiepId,
                CodeProject = "Project for plan " + value.PlanCode,
                SinglePlanProject = true
            };

            Project projectResponse = mapper.Map<ProjectModel, Project>(newProject);
            Project projectApplyChanges = await Add(projectResponse);
            ProjectModel projectModel = mapper.Map<Project, ProjectModel>(projectApplyChanges);
            logger.LogDebug("ProjectRepository end call CreateProjectForPlan -> return Repository response ProjectModel");

            return new RepositoryResponse<ProjectModel>(projectModel);
        }
    }
}

