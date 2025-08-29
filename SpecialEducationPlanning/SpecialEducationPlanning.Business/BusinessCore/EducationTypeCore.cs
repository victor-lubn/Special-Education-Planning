using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class HousingTypeCore
    {
        public static async Task<HousingType> GetHousingTypeAsync(this IEntityRepository<int> entityRepository,
            int housingTypeId,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingType>(p => p.Id == housingTypeId);
            var housingType = await entityRepository.Where(spec).AsNoTracking().SingleAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingType;
        }

        public static async Task<List<HousingType>> GetHousingTypeIncludeAsync(this IEntityRepository<int> entityRepository,
            ILogger logger)
        {
            logger.LogDebug("HousingSpecificationCore called GetHousingSpecificationAsync");

            var spec = new Specification<HousingType>(p => true);
            var housingType = await entityRepository.Where(spec).Include(x => x.Plans).AsNoTracking().ToListAsync();

            logger.LogDebug("HousingSpecificationCore end call GetHousingSpecificationAsync -> return HousingSpecification");

            return housingType;
        }

        public static async Task<bool> UpdateHousingTypeNameAsync(this IEntityRepository<int> entityRepository,
            int housingTypeId,
            string housingTypeName,
            DbContext context,
            ILogger logger)
        {
            logger.LogDebug("HousingTypeCore called UpdateHousingTypeNameAsync");

            var result = await context.Set<HousingType>()
                                .Where(x => x.Id == housingTypeId)
                                .ExecuteUpdateAsync(setter => setter
                                    .SetProperty(p => p.Name, housingTypeName));

            logger.LogDebug("HousingTypeCore end call UpdateHousingTypeNameAsync -> return boolean");

            if (result == 0)
            {
                return false;           
            }
            return true;
        }
    }
}
