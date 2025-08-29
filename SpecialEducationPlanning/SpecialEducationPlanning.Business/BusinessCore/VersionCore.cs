using Koa.Persistence.EntityRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.CatalogCodeSpecification;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public static class VersionCore
    {
        public static async Task<Version> CreateVersionAsync(this IEntityRepository entityRepository, DbContext dataContext,
            Plan plan, int versionId, VersionInfoModel versionInfoModel, ILogger logger)
        {
            logger.LogDebug("VersionCore called CreateVersionAsync");

            logger.LogDebug("Plan information: Code: {code} Range: {range}", versionInfoModel?.CatalogCode, versionInfoModel?.Range);
            var spec = new CatalogByCodeSpecification(versionInfoModel.CatalogCode, EducationOriginType.Fusion.GetDescription());
            var catalog = await entityRepository.Where(spec).SingleOrDefaultAsync();
            if (catalog.IsNull())
            {
                logger.LogError("Error uploading plans: no catalogue");

                logger.LogDebug("VersionCore end call CreateVersionAsync -> return Null");

                return null;
            }
            logger.LogDebug("Getting catalogs for {type}#{plan}", typeof(Plan), plan.Id);

            Version version = null;

            if (versionId == 0)
            {
                var sequenceVersionAiepCode = await entityRepository.GetNextExternalIdSequenceValueAsync(dataContext, plan.Id, logger);
                var versionNumber = await entityRepository.GetLastPlanVersionNumberAsync(plan.Id, logger);

                version = new Version()
                {
                    PlanId = plan.Id,
                    AiepCode = sequenceVersionAiepCode.Item2,
                    ExternalCode = sequenceVersionAiepCode.Item1,
                    VersionNumber = (!versionNumber.HasValue || versionNumber.Value == 0) ? 1 : versionNumber.Value + 1
                };

                // Set only first version to MasterVersion
                if (!plan.MasterVersionId.HasValue)
                {
                    plan.MasterVersion = version;
                }

                plan.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                version = await entityRepository.Where(new EntityByIdSpecification<Version>(versionId)).Include(v => v.RomItems).FirstOrDefaultAsync();
            }

            if (version.IsNull())
            {
                logger.LogError("Error getting or creating version");

                logger.LogDebug("VersionCore end call CreateVersionAsync -> return Null");

                return null;
            }

            logger.LogDebug("Version #{version} created for {type}#{plan}", version.Id, typeof(Plan), plan.Id);
            version.RomItems.Clear();

            version.CatalogId = catalog.Id;
            version.VersionNotes = versionInfoModel.VersionNotes;
            version.QuoteOrderNumber = versionInfoModel.QuoteOrderNumber;
            version.Range = versionInfoModel.Range;

            logger.LogDebug("Creating ROM items for {type}#{plan}", version.Id, typeof(Plan), plan.Id);
            foreach (var romItem in versionInfoModel.RomItems)
            {
                var newRomItem = new RomItem()
                {
                    ItemName = romItem.ItemName,
                    CatalogId = catalog.Id,
                    Range = romItem.Range,
                    Colour = romItem.Colour,
                    Qty = romItem.Qty,
                    VersionId = version.Id,
                    Description = romItem.Description,
                    Handing = romItem.Handing,
                    PosNumber = romItem.PosNumber,
                    Annotation = romItem.Annotation,
                    OrderCode = romItem.OrderCode
                };

                version.RomItems.Add(newRomItem);
            }

            logger.LogDebug("VersionCore end call CreateVersionAsync -> return Version");

            return version;
        }

        /// <summary>
        /// Get Next Value of the sequence for the External ID
        /// </summary>
        /// <returns>string</returns>
        public static async Task<Tuple<string, string>> GetNextExternalIdSequenceValueAsync(this IEntityRepository entityRepository, DbContext dataContext, int planId,
            ILogger logger)
        {
            logger.LogDebug("VersionCore called GetNextExternalIdSequenceValueAsync");

            var sequence = dataContext.GetNextExternalIdSequenceValueAsync();

            var planWithParents = await entityRepository.Where(new EntityByIdSpecification<Plan>(planId))
               .IgnoreQueryFilters()
               .Include(plan => plan.Project)
               .ThenInclude(project => project.Aiep)
               .FirstOrDefaultAsync();

            if (planWithParents == null
                || planWithParents.Project == null
                || planWithParents.Project.Aiep == null)
            {
                logger.LogDebug("VersionCore end call GetNextExternalIdSequenceValueAsync -> return Null");

                return null;
            }
            var AiepCode = planWithParents.Project.Aiep.AiepCode;

            logger.LogDebug("VersionCore end call GetNextExternalIdSequenceValueAsync -> return Tuple<String, String>");

            return new Tuple<string, string>(AiepCode.Substring(AiepCode.Length - 3) + sequence, AiepCode);
        }

        public static async Task<int?> GetLastPlanVersionNumberAsync(this IEntityRepository entityRepository, int planId,
            ILogger logger)
        {
            logger.LogDebug("VersionCore called GetLastPlanVersionNumberAsync");

            var spec = new Domain.Specification.VersionSpecifications.VersionsByPlanIdSpecification(planId);
            var result = (await entityRepository.Where(spec).IgnoreQueryFilters().OrderByDescending(x => x.VersionNumber).FirstOrDefaultAsync())?.VersionNumber;

            logger.LogDebug("VersionCore end call GetLastPlanVersionNumberAsync -> return Int");

            return result;
        }
    }
}

