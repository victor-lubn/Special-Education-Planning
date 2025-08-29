using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class ProjectCore
    {
        public static Project CreateProjectForAPlan(this IEntityRepository<int> entityRepository,
            string planCode, int AiepId, ILogger logger)
        {
            logger.LogDebug("ProjectCore called CreateProjectForAPlan");

            var project = new Project
            {
                AiepId = AiepId,
                CodeProject = "Project for plan " + planCode,
                SinglePlanProject = true
            };

            entityRepository.Add(project);

            logger.LogDebug("Project end call CreateProjectForAPlan -> return Project");

            return project;
        }

        public static async Task<Project> UpdateProjectWithHousingSpecsAsync(this IEntityRepository<int> entityRepository,
            HousingSpecification housingSpecs, int projectId, DbContext context, ILogger logger)
        {
            logger.LogDebug("ProjectCore called UpdateProjectWithHousingSpecsAsync");

            var spec = new Specification<Project>(p => p.Id == projectId);
            var project = await entityRepository.Where(spec)
                                                 .Include(x => x.HousingSpecifications)
                                                 .SingleAsync();
            if (project.HousingSpecifications.Any(x => x.Code.Equals(housingSpecs.Code) && 
                                                       x.Name.Equals(housingSpecs.Name))) 
            {
                return project;
            }
            housingSpecs.Id = 0;
            project.HousingSpecifications.Add(housingSpecs);
            context.SaveChanges();

            logger.LogDebug("Project end call UpdateProjectWithHousingSpecsAsync -> return Project");

            return project;
        }

        public static async Task<bool> IsProjectArchived(this IEntityRepository<int> entityRepository,
            int projectId,
            ILogger logger)
        {
            logger.LogDebug("PlanCore called IsProjectArchived");

            var spec = new Specification<Project>(p => p.Id == projectId && (p.IsArchived ?? false));
            var isProjectArchived = await entityRepository.Where(spec).AnyAsync();

            logger.LogDebug("PlanCore end call IsProjectArchived -> return boolean");

            return isProjectArchived;
        }

    }
}

