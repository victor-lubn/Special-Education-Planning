using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Api.Configuration.DataMigration;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;

namespace SpecialEducationPlanning
.Api.Services.DataMigration
{

    /// <summary>
    ///     Plan migration service
    /// </summary>
    public class PlanMigrationService : MigrationBaseService<Plan, QuoteMigrationModel, string>
    {

        /// <summary>
        ///     Cache boosting
        /// </summary>
        private static int defaultCatalogId;

        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;

        private readonly IPlanRepository planRepository;
        private IOptions<DataMigrationConfiguration> dataMigrationConfigurationOptions;
        private List<EndUser> endUsers;

        private CadFileUnzip zipHelper = new CadFileUnzip();

        /// <summary>
        ///     Createas a new instance of <see cref="PlanMigrationService" />
        /// </summary>
        /// <param name="dbContextAccesor">DbContext accessor</param>
        /// <param name="logger">Logger</param>
        /// <param name="fileStorageService">Azure file storage service</param>
        public PlanMigrationService(IDistributedCache cache, IDbContextAccessor dbContextAccesor,
            ILogger<MigrationBaseService<Plan, QuoteMigrationModel, string>> logger,
            IFileStorageService<AzureStorageConfiguration> fileStorageService, IPlanRepository planRepository, IIdentityProvider identityProvider,
            DbContextOptions dbContextOptions, ILogger<DataContext> dataContextLogger, IObjectMapper mapper, IOptions<DataMigrationConfiguration> options) : base(cache, dbContextAccesor,
            logger, identityProvider, dbContextOptions, dataContextLogger, mapper, false)
        {
            this.fileStorageService = fileStorageService;
            this.planRepository = planRepository;
            this.dataMigrationConfigurationOptions = options;
        }

        #region Methods Protected

        protected override EntityEntry DetachEntity<K>(K entity)
        {
            var entityEntry = base.DetachEntity(entity);
            var plan = entity as Plan;

            if (plan == null)
            {
                return entityEntry;
            }

            base.DetachEntity(plan.Project);

            if (plan.EndUser != null)
            {
                base.DetachEntity(plan.EndUser);
            }

            foreach (var version in plan.Versions)
            {
                base.DetachEntity(version);
            }

            return entityEntry;
        }

        /// <summary>
        ///     No secondary key
        /// </summary>
        /// <param name="query">Query to filter quotes</param>
        /// <param name="model">Model to use as filter</param>
        /// <returns></returns>
        protected override Plan FilterBySecondaryKey(IQueryable<Plan> query, QuoteMigrationModel model)
        {
            return null;
        }

        /// <summary>
        ///     This migration filter is only the TdpId sent by the migration server
        /// </summary>
        /// <param name="models">Models to use as source map</param>
        /// <returns>Plans that matches the criteria</returns>
        protected override IEnumerable<Plan> GetEntities(IEnumerable<QuoteMigrationModel> models)
        {
            var addresses = models.Select(ComposeAddress1);
            var surnames = models.Select(x => x.ConsumerSurname);
            var postcodes = models.Select(x => x.ConsumerPostCode.NormalisePostcode());

            endUsers = DbSet<EndUser>().Where(eu =>
                    addresses.Contains(eu.Address1) &&
                    surnames.Contains(eu.Surname) &&
                    postcodes.Contains(eu.Postcode)).Include(eu => eu.EndUserAieps)
                .ToList();

            return new List<Plan>();
        }

        /// <summary>
        ///     Creates the plan lookup
        /// </summary>
        /// <param name="entities">Entitites to build the lookup against</param>
        /// <returns>Plan lookup</returns>
        protected override EntityBaseLookup<Plan, int, string> GetLookup(IEnumerable<Plan> entities)
        {
            return new PlanLookup(entities);
        }

        /// <summary>
        ///     Returns QuoteNmbr as secondary key
        /// </summary>
        /// <param name="model">Model to migrate</param>
        /// <returns>Secondary key</returns>
        protected override string GetSecondaryKey(QuoteMigrationModel model)
        {
            return model.QuoteNmbr.ToString();
        }

        /// <summary>
        ///     Maps a quote with a Plan/Model, uploading the plan and preview to Azure
        /// </summary>
        /// <param name="model">quote to migrate</param>
        /// <param name="entity">Plan in TDp</param>
        /// <returns></returns>
        protected override Plan Map(QuoteMigrationModel model, Plan entity)
        {
            entity = base.Map(model, entity);
            MapRelationships(model, entity);
            entity.PlanCode = planRepository.GeneratePlanIdAsync(entity.UpdatedDate).GetAwaiter().GetResult().Content;

            return entity;
        }

        /// <summary>
        ///     When mapping back, we send TdpVersionId as well
        /// </summary>
        /// <param name="entity">Entity to map from</param>
        /// <param name="model">Model to map to</param>
        protected override void MapBack(Plan entity, QuoteMigrationModel model)
        {
            base.MapBack(entity, model);
            model.TdpVersionId = entity.Versions.First().Id;
        }

        #endregion

        #region Methods Private

        private string ComposeAddress1(QuoteMigrationModel model)
        {
            return new List<string>
                    {model.ConsumerPlot, model.ConsumerHouse, model.ConsumerHouseNo, model.ConsumerAddress1}
                .ComposeAddress();
        }

        /// <summary>
        ///     Creates the catalog
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void CreateCatalog(QuoteMigrationModel model, Plan entity)
        {
            if (defaultCatalogId == 0)
            {
                var catalog = DbSet<Catalog>().AsNoTracking().First();
                defaultCatalogId = catalog.Id;
            }

            entity.CatalogId = defaultCatalogId;

            //entity.CatalogId = defaultCatalogId;
            //if (entity.CatalogId == 0)
            //{
            //    var catalog = Mapper.Map(model, new Catalog());
            //    entity.Catalog = catalog;
            //    Add(catalog);
            //    SaveChanges();
            //    entity.CatalogId = catalog.Id;
            //}
        }

        /// <summary>
        ///     Check if an end user exists and adds it to the given plan
        /// </summary>
        /// <param name="model">Model to migrate the end user from</param>
        /// <param name="entity">Plan to migrate the end user to</param>
        private void CreateEndUser(QuoteMigrationModel model, Plan entity)
        {
            var AiepId = Convert.ToInt32(model.AiepTdpId);
            var address1 = ComposeAddress1(model);
            var postcode = model.ConsumerPostCode.NormalisePostcode();

            if (string.IsNullOrWhiteSpace(address1?.Trim()) || string.IsNullOrWhiteSpace(postcode?.Trim()) ||
                string.IsNullOrWhiteSpace(model.ConsumerSurname?.Trim()))
            {
                if (!string.IsNullOrEmpty(dataMigrationConfigurationOptions.Value.PlanWarningStringMandatoryCreateEndUser))
                {
                    if (!string.IsNullOrEmpty(model.Message)) model.Message += Environment.NewLine;
                    model.Message += string.Format(dataMigrationConfigurationOptions.Value.PlanWarningStringMandatoryCreateEndUser, address1, postcode, model?.ConsumerSurname);
                }

                return;
            }

            var endUser = endUsers.FirstOrDefault(x =>
                              x.Address1 == address1 && x.Surname == model.ConsumerSurname && x.Postcode == postcode) ??
                          DbSet<EndUser>().Where(x =>
                                  x.Address1 == address1 && x.Surname == model.ConsumerSurname &&
                                  x.Postcode == postcode)
                              .Include(eu => eu.EndUserAieps).FirstOrDefault();

            if (endUser != null && endUser.Id > 0) //Found on DB
            {
                this.mapper.Map(model, endUser);
                entity.EndUserId = endUser.Id;

                if (!DbSet<EndUserAiep>().AsNoTracking().Any(x => x.EndUserId == endUser.Id && x.AiepId == AiepId))
                {
                    Add(new EndUserAiep { EndUserId = endUser.Id, AiepId = model.AiepTdpId });
                }
            }
            else if (endUser != null && endUser.Id <= 0) //Found on memory not saved on DB
            {
                this.mapper.Map(model, endUser);
                entity.EndUser = endUser;

                if (!endUser.EndUserAieps.Any(eud => eud.AiepId == AiepId))
                {
                    var eud = new EndUserAiep { EndUser = endUser, AiepId = model.AiepTdpId };
                    endUser.EndUserAieps.Add(eud);
                }
            }
            else //Brand new
            {
                endUser = new EndUser();
                this.mapper.Map(model, endUser);
                entity.EndUser = endUser;
                Add(new EndUserAiep { EndUser = endUser, AiepId = model.AiepTdpId });
            }
        }

        /// <summary>
        ///     Creates the project
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void CreateProject(QuoteMigrationModel model, Plan entity)
        {
            if (entity.ProjectId == 0)
            {
                entity.Project = new Project
                {
                    AiepId = model.AiepTdpId,
                    SinglePlanProject = true,
                    CreatedDate = DateTime.Now
                };
            }
        }

        /// <summary>
        ///     Creates a new version
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="romPath">Task uploading the ROM file</param>
        /// <param name="previewPath">Task uploading the preview</param>
        /// <returns></returns>
        private void CreateVersion(QuoteMigrationModel model, Plan entity)
        {
            var version = new Version
            {
                RomPath = model.Plan,
                PreviewPath = model.PlanPreview,
                CatalogId = entity.CatalogId,
                ExternalCode = entity.CadFilePlanId,
                Plan = entity,
                VersionNumber = 1,
                VersionNotes = GetVersionNotes(model, dataMigrationConfigurationOptions.Value.PlanVersionNotesTemplate),
                AiepCode = model.AiepFile,
                Range = model.Range
            };

            entity.Versions.Add(version);
            entity.CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        ///     Given the model, creates the version notes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static string GetVersionNotes(QuoteMigrationModel model, string planVersionNotesTemplate)
        {
            const int consumerCommentsLength = 500;
            var versionNotes = "";
            if (!string.IsNullOrEmpty(planVersionNotesTemplate))
            {
                versionNotes = string.Format(planVersionNotesTemplate,
                   DateTime.Now,
                   model.Educationer,
                   model.Catalogue,
                   model.PlanDate,
                   model.PlanCreationDate,
                   !model.ConsumerComments.IsNullOrEmpty() ? model.ConsumerComments : "N/A");
            }

            if (versionNotes.Length > consumerCommentsLength)
            {
                versionNotes = versionNotes.Substring(0, consumerCommentsLength);
            }

            return versionNotes;
        }

        /// <summary>
        ///     Map relationships with other tables
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void MapRelationships(QuoteMigrationModel model, Plan entity)
        {
            CreateCatalog(model, entity);
            CreateProject(model, entity);
            CreateEndUser(model, entity);
            CreateVersion(model, entity);
        }

        /// <summary>
        ///     Upload a file to Azure
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        private async Task<string> UploadFile<T>(byte[] fileContent)
        {
            if (fileContent.Length <= 0)
            {
                return null;
            }

            using (var stream = new MemoryStream(fileContent))
            {
                return await fileStorageService.UploadAsync<T>(stream);
            }
        }

        #endregion

    }

}

