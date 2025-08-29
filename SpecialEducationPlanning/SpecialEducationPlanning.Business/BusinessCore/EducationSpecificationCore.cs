using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class HousingSpecificationCore
    {
        public static async Task<HousingSpecification> GetHousingSpecificationAsync(this IEntityRepository<int> entityRepository,
            int housingSpecId,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingSpecification>(p => p.Id == housingSpecId);
            var housingSpec = await entityRepository.Where(spec).AsNoTracking().SingleAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingSpec;
        }

        public static async Task<HousingSpecification> GetHousingSpecificationAsync(this IEntityRepository<int> entityRepository,
            int projectId,
            string housingSpecCode,
            string housingSpecName,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingSpecification>(p => p.ProjectId == projectId && 
                                                                    p.Code.Equals(housingSpecCode) && 
                                                                    p.Name.Equals(housingSpecName));
            var housingSpec = await entityRepository.Where(spec).AsNoTracking().SingleAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingSpec;
        }

        public static async Task<bool> UpdateHousingSpecificationNameAsync(this IEntityRepository<int> entityRepository,
            int housingSpecificationId,
            string housingSpecificationName,
            DbContext context,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called UpdateHousingSpecificationNameAsync");

            var result = await context.Set<HousingSpecification>()
                            .Where(x => x.Id == housingSpecificationId)
                            .ExecuteUpdateAsync(setter => setter
                                .SetProperty(p => p.Name, housingSpecificationName));

            logger.LogDebug("HousingSpecificationCore end call UpdateHousingSpecificationNameAsync -> return boolean");
   
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> HasProjectHousingSpecificationAsync(this IEntityRepository<int> entityRepository,
            int projectId,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called HasProjectHousingSpecificationAsync");

            var spec = new Specification<HousingSpecification>(p => p.ProjectId == projectId);
            var hasSpec = await entityRepository.AnyAsync(spec);

            logger.LogDebug($"HousingSpecificationCore end call HasProjectHousingSpecificationAsync -> return {hasSpec}");

            return hasSpec;
        }
    }
}
