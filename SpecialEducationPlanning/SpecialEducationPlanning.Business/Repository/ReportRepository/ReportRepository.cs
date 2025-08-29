using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Report;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Repository
{

    public class ReportRepository : BaseRepository<Aiep>, IReportRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;

        private readonly ILogger<ReportRepository> logger;

        public ReportRepository(ILogger<ReportRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository entityRepository) :
            base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;

            this.logger = logger;
        }

        #region Methods IReportRepository

        public async Task<RepositoryResponse<ICollection<AiepReportModel>>> GetReport(DateTime fromDate,
            DateTime toDate)
        {
            logger.LogDebug("ReportRepository called GetReport");

            ICollection<AiepReportModel> reportModel = new List<AiepReportModel>();

            var Aieps = await entityRepositoryKey
                .GetAll<Aiep>()
                .Include(d => d.Educationers)
                .Include(dp => dp.BuilderEducationerAieps).ThenInclude(bdp => bdp.Builder)
                .Include(dp => dp.Projects).ThenInclude(pr => pr.Plans).ThenInclude(pl => pl.Catalog)
                .Include(dp => dp.Projects).ThenInclude(pr => pr.Plans).ThenInclude(pl => pl.Versions)
                .Include(dp => dp.Projects).ThenInclude(pr => pr.Plans).ThenInclude(pl => pl.EndUser)
                .Include(dp => dp.Projects).ThenInclude(pr => pr.Plans).ThenInclude(pl => pl.Versions).ThenInclude(v => v.RomItems)
                .Include(dp => dp.Projects).ThenInclude(pr => pr.Plans).ThenInclude(pl => pl.Versions).ThenInclude(v => v.Catalog)
                .Where(dp =>
                    dp.Projects.Any(pr => pr.Plans.Any(pl => pl.UpdatedDate >= fromDate && pl.UpdatedDate <= toDate)))
                .ToListAsync();

            foreach (var Aiep in Aieps)
            {
                var plans = Aiep.Projects
                    .SelectMany(pr => pr.Plans.Where(pl => pl.UpdatedDate >= fromDate && pl.UpdatedDate <= toDate))
                    .ToList();

                if (!plans.Any())
                {
                    continue;
                }

                var AiepReportModel = new AiepReportModel
                {
                    AiepCode = Aiep.AiepCode
                };

                AiepReportModel.Builders.AddRange(GetBuildersFromAiep(Aiep.BuilderEducationerAieps));
                AiepReportModel.Educationers.AddRange(GetEducationersFromAiep(Aieps));
                AiepReportModel.Plans.AddRange(GetPlansFromAiep(plans));
                reportModel.Add(AiepReportModel);
            }

            logger.LogDebug("ReportRepository end call GetReport -> return Repository response Collection of AiepReportModel");

            return new RepositoryResponse<ICollection<AiepReportModel>>(reportModel);
        }

        #endregion

        #region Methods Private

        private List<BuilderReportModel> GetBuildersFromAiep(ICollection<BuilderEducationerAiep> builderEducationerAieps)
        {
            logger.LogDebug("ReportRepository called GetBuildersFromAiep");

            var builderReportList = new List<BuilderReportModel>();

            foreach (var builderEducationerAiep in builderEducationerAieps)
            {
                var builder = builderEducationerAiep.Builder;

                if (builder.IsNotNull())
                {
                    builderReportList.Add(new BuilderReportModel
                    {
                        Id = builder.Id,
                        AccountNumber = builder.AccountNumber,
                        Address0 = builder.Address0,
                        Address1 = builder.Address1,
                        Address2 = builder.Address2,
                        Address3 = builder.Address3,
                        Email = builder.Email,
                        LandLineNumber = builder.LandLineNumber,
                        Name = builder.Name,
                        Notes = builder.Notes,
                        Postcode = builder.Postcode,
                        TradingName = builder.TradingName,
                        SAPAccountStatus = builder.SAPAccountStatus,
                        BuilderStatus=builder.BuilderStatus
                    });
                }
            }

            logger.LogDebug("ReportRepository end call GetBuildersFromAiep -> return List of BuilderReportModel");

            return builderReportList;
        }

        private IEnumerable<EducationerReportModel> GetEducationersFromAiep(ICollection<Aiep> Aieps)
        {
            logger.LogDebug("ReportRepository called GetEducationersFromAiep");

            logger.LogDebug("ReportRepository end call GetEducationerFromAiep -> return call Aieps.SelectMany.ToInmutableHashSet");

            return Aieps.SelectMany(d => d.Educationers.Select(de =>
                new EducationerReportModel
                {
                    FullName = de.FirstName + " " + de.Surname,
                    Id = de.Id
                })
            ).ToImmutableHashSet();
        }

        private List<PlanReportModel> GetPlansFromAiep(List<Plan> plans)
        {
            logger.LogDebug("ReportRepository called GetPlansFromAiep");

            var planReportList = new List<PlanReportModel>();

            foreach (var plan in plans)
            {
                if (plan.IsNotNull())
                {
                    var planreportmodel = new PlanReportModel
                    {
                        CreatedDate = plan.CreatedDate,
                        UpdatedDate = plan.UpdatedDate,
                        Survey = plan.Survey,
                        EndUser = new EndUserReportModel
                        {
                            Postcode = plan.EndUser?.Postcode ?? "",
                            Firstname = plan.EndUser?.FirstName ?? "",
                            Surname = plan.EndUser?.Surname ?? ""
                        },
                        Id = plan.PlanCode,
                        BuilderId = plan.BuilderId,
                        EducationerId = plan.EducationerId
                    };

                    if (plan.MasterVersion.IsNotNull())
                    {
                        if (plan.MasterVersion.Catalog.IsNotNull())
                        {
                            planreportmodel.Catalog = new CatalogReportModel
                            {
                                Code = plan.MasterVersion.Catalog.Code,
                                Name = plan.MasterVersion.Catalog.Name
                            };

                            var romItems = new List<RomItemReportModel>();

                            foreach (var item in plan.MasterVersion.Catalog.RomItems)
                            {
                                romItems.Add(new RomItemReportModel
                                {
                                    Name = item.ItemName,
                                    Sku = item.Sku,
                                    Quantity = item.Qty,
                                    VersionId = item.VersionId
                                });
                            }

                            planreportmodel.RomItems = romItems;
                        }
                    }

                    planReportList.Add(planreportmodel);
                }
            }

            logger.LogDebug("ReportRepository end call GetPlansFromAiep -> return List of PlanReportModel");

            return planReportList;
        }

        #endregion

    }

}

