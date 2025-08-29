using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.AiepSpecifications;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class AiepCore
    {
        public static async Task<Aiep> GetAiepWithEducationersAsync(this IEntityRepository<int> entityRepository,
            int AiepId, ILogger logger)
        {
            logger.LogDebug("AiepCore called GetAiepWithEducationersAsync");

            logger.LogDebug("AiepCore end call GetAiepWithEducationersAsync -> return Aiep");

            return await entityRepository.Where(new EntityByIdSpecification<Aiep>(AiepId))
                .Include(d => d.Educationers).FirstOrDefaultAsync();
        }

        public static async Task<Aiep> GetAiepByAiepCode(this IEntityRepository<int> entityRepository,
            string AiepCode, ILogger logger)
        {
            logger.LogDebug("AiepCore called GetAiepByAiepCode");

            logger.LogDebug("AiepCore end call GetAiepByAiepCode -> return Aiep");

            return await entityRepository.Where(new AiepByKeyNameSpecification(AiepCode)).FirstOrDefaultAsync();
        }

        public static async Task<Aiep> CreateOrUpdateAiep(this IEntityRepository<int> entityRepository, AiepModel AiepModel, IObjectMapper mapper,
            ILogger logger)
        {
            logger.LogDebug("AiepCore called CreateOrUpdateAiep");

            Aiep Aiep = null;

            if (AiepModel.Id == 0)
            {
                Aiep = new Aiep();
            }
            else
            {
                Aiep = await entityRepository.Where(new EntityByIdSpecification<Aiep>(AiepModel.Id)).FirstOrDefaultAsync();
            }

            AiepModel.Postcode = AiepModel.Postcode.NormalisePostcode();
            Aiep = mapper.Map<AiepModel, Aiep>(AiepModel, Aiep);

            logger.LogDebug("AiepCore end call CreateOrUpdateAiep -> return Aiep");

            return Aiep;
        }

        public static async Task<Aiep> GetAiepByAiepCodeIgnoreFilter(this IEntityRepository<int> entityRepository,
            string AiepCode, ILogger logger)
        {
            logger.LogDebug("AiepCore called GetAiepByAiepCodeIgnoreFilter");

            logger.LogDebug("AiepCore end call GetAiepByAiepCodeIgnoreFilter -> return Aiep");

            return await entityRepository.Where(new AiepByKeyNameSpecification(AiepCode)).IgnoreQueryFilters().FirstOrDefaultAsync();
        }
    }
}

