using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification.BuilderSpecifications;
using SpecialEducationPlanning
.Domain.Specification.CustomerSpecifications;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class BuilderCore
    {
        public static void DemoBuilderCoreFunction(this IEntityRepository<int> entityRepository)
        {
        }

        public static async Task<Builder> GetBuilderNoAclAsymc(this IEntityRepository<int> entityRepository, int builderId, ILogger logger)
        {
            logger.LogDebug("BuilderCore called GetBuilderNoAclAsymc");

            logger.LogDebug("BuilderCore end call GetBuilderNoAclAsymc -> return Builder");

            return await entityRepository.GetAll<Builder>().Where(b => b.Id == builderId).IgnoreQueryFilters().FirstOrDefaultAsync();
        }

        public static async Task<Builder> GetExistingBuilderOnlyAccountNumberAsync(this IEntityRepository<int> entityRepository, string accountNumber, ILogger logger)
        {
            logger.LogDebug("BuilderCore called GetExistingBuilderOnlyAccountNumberAsync");

            var builderSpec = new IsExistingBuilderByAccountNumberSpec(accountNumber);

            var existingBuilder = await entityRepository.Where(builderSpec).IgnoreQueryFilters().SingleOrDefaultAsync();

            logger.LogDebug("BuilderCore end call GetExistingBuilderOnlyAccountNumberAsync -> return Builder");

            return existingBuilder;
        }

        public static async Task<Builder> GetExistingBuilderOnlyMandatoryFieldsAsync(this IEntityRepository<int> entityRepository, Builder builder, ILogger logger)
        {
            logger.LogDebug("BuilderCore called GetExistingBuilderOnlyMandatoryFieldsAsync");

            var builderSpec = new BuilderByMandatoryFieldsSpecification(builder.TradingName, builder.Postcode, builder.Address1);
            var oldestOfBuilders = await entityRepository.Where(builderSpec).IgnoreQueryFilters().OrderBy(x => x.CreatedDate).FirstOrDefaultAsync();

            logger.LogDebug("BuilderCore end call GetExistingBuilderOnlyMandatoryFieldsAsync -> return Builder");

            return oldestOfBuilders;
        }
    }



}
