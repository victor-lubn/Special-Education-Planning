

using Koa.Persistence.EntityRepository;
using Microsoft.Extensions.Logging;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class HousingSpecificationTemplateCore
    {
        public static HousingSpecificationTemplates CreateHousingSpecificationTemplate(this IEntityRepository<int> entityRepository, int housingSpecificationId, int planId, ILogger logger)
        {
            logger.LogDebug("ProjectTemplateCore called CreateProjectTemplateForAPlan");

            var housingSpecificationTemplate = new HousingSpecificationTemplates
            {
                HousingSpecificationId = housingSpecificationId,
                PlanId = planId
            };

            entityRepository.Add(housingSpecificationTemplate);

            logger.LogDebug("ProjectTemplateCore end call CreateProjectTemplateForAPlan -> return ProjectTemplates");

            return housingSpecificationTemplate;
        }
    }
}
