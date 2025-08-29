using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class ProjectTemplateCore
    {
        public static ProjectTemplates CreateProjectTemplate(this IEntityRepository<int> entityRepository,
            int projectId, int planId, ILogger logger)
        {
            logger.LogDebug("ProjectTemplateCore called CreateProjectTemplateForAPlan");

            var projectTemplate = new ProjectTemplates
            {
                ProjectId = projectId,
                PlanId = planId
            };

            entityRepository.Add(projectTemplate);

            logger.LogDebug("ProjectTemplateCore end call CreateProjectTemplateForAPlan -> return ProjectTemplates");

            return projectTemplate;
        }
    }
}
