using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Platform.Providers.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Model.SapServiceModel;
using SpecialEducationPlanning
.Api.Service.Sap;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel.Lookups;
using SpecialEducationPlanning
.Domain;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions;

namespace SpecialEducationPlanning
.Api.Services.DataMigration
{

    /// <summary>
    ///     Customer migration service
    /// </summary>
    public class BuilderMigrationService : MigrationBaseService<Builder, CustomerMigrationModel, string>
    {

        private readonly ILogger<MigrationBaseService<Builder, CustomerMigrationModel, string>> logger;

        private readonly ISapService sapService;
        private List<BuilderModel> sapBuilders;

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderMigrationService" />
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="dbContextAccesor">Database accessor</param>
        /// <param name="logger">Logger</param>
        /// <param name="sapService"></param>
        /// <param name="identityProvider"></param>
        /// <param name="dbContextOptions"></param>
        /// <param name="dataContextLogger"></param>
        /// <param name="mapper"></param>
        public BuilderMigrationService(IDistributedCache cache, IDbContextAccessor dbContextAccesor,
            ILogger<MigrationBaseService<Builder, CustomerMigrationModel, string>> logger, ISapService sapService,
            IIdentityProvider identityProvider, DbContextOptions dbContextOptions,
            ILogger<DataContext> dataContextLogger, IObjectMapper mapper) : base(cache, dbContextAccesor,
            logger, identityProvider, dbContextOptions, dataContextLogger, mapper)
        {
            this.logger = logger;
            this.sapService = sapService;
        }

        #region Methods Protected

        protected override EntityEntry DetachEntity<K>(K entity)
        {
            var entityEntry = base.DetachEntity(entity);

            if (!(entity is Builder builder))
            {
                return entityEntry;
            }

            foreach (var builderBuilderEducationerAiep in builder.BuilderEducationerAieps)
            {
                base.DetachEntity(builderBuilderEducationerAiep);
            }

            return entityEntry;
        }

        /// <summary>
        ///     Looks for a builder whose TradingName, Address1 and Postcode matches the given model
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="model">Model to use as filter</param>
        /// <returns></returns>
        protected override Builder FilterBySecondaryKey(IQueryable<Builder> query, CustomerMigrationModel model)
        {
            // This criteria should be the same that the one applied in CustomerMigrationModelProfile!!
            var address1 = GetFinalAddress(model);
            var modelPostCode = model.Postcode.NormalisePostcode();

            return query.Where(x => x.TradingName == model.Surname &&
                                             x.Address1 == address1 && x.Postcode == modelPostCode).Include(x => x.BuilderEducationerAieps).FirstOrDefault();
        }

        /// <summary>
        ///     Given the firstname, lastname and postcode, tries to get the information from TDP database
        /// </summary>
        /// <param name="models">Models to map</param>
        /// <returns>Entities matching the criteria</returns>
        protected override IEnumerable<Builder> GetEntities(IEnumerable<CustomerMigrationModel> models)
        {
            var sapDisabled = models.All(m => m.DisableSapValidation);
            var normalizedModels = NormalizeModels(models);

            if (!sapDisabled)
            {
                sapBuilders = GetSapEntities(normalizedModels);
            }

            if (!avoidDuplicates)
            {
                return new List<Builder>();
            }

            var addresses = normalizedModels.Select(x => x.FinalAddress);
            var surnames = normalizedModels.Select(x => x.Customer.Surname);
            var postcodes = normalizedModels.Select(x => x.NormalizedPostcode);

            return DbSet<Builder>().Where(b =>
                    addresses.Contains(b.Address1) &&
                    surnames.Contains(b.TradingName) &&
                    postcodes.Contains(b.Postcode)).Include(b => b.BuilderEducationerAieps)
                .ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string GetFinalAddress(CustomerMigrationModel model)
        {
            return new List<string> { model.HouseName, model.Address1 }.ComposeAddress();
        }

        /// <summary>
        ///     Returns a new builder lookup for the given entities
        /// </summary>
        /// <param name="entities">Entities to lookup</param>
        /// <returns>Lookup</returns>
        protected override EntityBaseLookup<Builder, int, string> GetLookup(IEnumerable<Builder> entities)
        {
            return new BuilderLookup(entities);
        }

        /// <summary>
        ///     Creates a secondary lookup key comining Surname, Firstname and Postcode
        /// </summary>
        /// <param name="model">Model to get the key</param>
        /// <returns>Secondary key</returns>
        protected override string GetSecondaryKey(CustomerMigrationModel model)
        {
            var address1 = GetFinalAddress(model);
            var modelPostCode = model.Postcode.NormalisePostcode();

            return string.Join("-", model.Surname, address1, modelPostCode);
        }

        /// <summary>
        ///     Maps a CADFile with a TDP Builder.
        ///     The process consists of two stages:
        ///     Stage 1: We try to find a SAP builder. If exists, a lookup against to TDP is performed by AccountNumber. If the
        ///     Builder exists, is returned to Exodus.
        ///     Stage 2: If no SAP account exists for this builder, a new Builder is created in TDP.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override Builder Map(CustomerMigrationModel model, Builder entity)
        {
            // Optional flag for dev purposes
            var sapModel = model.DisableSapValidation ? null : GetBuilderFromSAP(model);

            // Business requirement: If the builder is not in SAP: Migrate it as cash builder and add CADFILE account number as a note
            // We use the base.Map function to create a new Builder
            if (sapModel.IsNull())
            {
                entity = base.Map(model, entity);
                entity.AccountNumber = null;
                entity.Notes = model.AccountNumber;
            }

            // If we have a SAP model, we need to get the equivalent Builder in TDP.
            else
            {
                entity = GetSapBuilderInTdp(sapModel, entity);
            }

            // Map relationships as usual
            MapRelationships(model, entity);

            return entity;
        }

        #endregion

        #region Methods Private

        /// <summary>
        ///     Executes a query against the SAP service to look for a Builder
        /// </summary>
        /// <param name="model">Exodus model</param>
        /// <returns>BuilderModel or null</returns>
        private BuilderModel GetBuilderFromSAP(CustomerMigrationModel model)
        {
            var accountSap = sapBuilders.FirstOrDefault(sb => sb.AccountNumber == model.AccountNumber);

            if (accountSap != null)
            {
                return accountSap;
            }

            var (NormalizedPostcode, FinalAddress, Customer) = NormalizeModels(new List<CustomerMigrationModel> { model }).First();

            return sapBuilders.FirstOrDefault(sb =>
                sb.Postcode == NormalizedPostcode.RepresentUKPostcode() &&
                sb.Address1 == FinalAddress && sb.TradingName == Customer.Surname);
        }

        /// <summary>
        ///     Given a SAP Model, tries to find Builder whose AccountNumber matches in TDP.
        ///     A new Builder is created if no match is found
        /// </summary>
        /// <param name="sapModel">SAP model found</param>
        /// <param name="entity">Entity from TDP database</param>
        /// <returns></returns>
        private Builder GetSapBuilderInTdp(BuilderModel sapModel, Builder entity)
        {
            sapModel.Postcode = sapModel.Postcode.NormalisePostcode();

            var entityAccounts = DbSet<Builder>().Where(x => x.Address1 == sapModel.Address1 &&
                                                             x.Postcode == sapModel.Postcode &&
                                                             x.TradingName == sapModel.TradingName).ToList();

            if (entityAccounts.Any())
            {
                entity = entityAccounts.FirstOrDefault();

                return mapper.Map(sapModel, entity);
            }

            return mapper.Map(sapModel, new Builder());
        }

        private List<BuilderModel> GetSapEntities(
            List<(string NormalizedPostcode, string FinalAddress, CustomerMigrationModel Customer)> normalizedModels)
        {
            var accountNumbers = new List<string>();
            var sapMandatoryFields = new List<SapByMandatoryFields>();

            foreach (var (NormalizedPostcode, FinalAddress, Customer) in normalizedModels.Where(nm => !nm.Customer.AccountNumber.IsNullOrEmpty())
                .ToList())
            {
                accountNumbers.Add(Customer.AccountNumber);
            }

            var i = 0;

            foreach (var (NormalizedPostcode, FinalAddress, Customer) in normalizedModels.Where(nm =>
                !nm.FinalAddress.IsNullOrEmpty() && !nm.NormalizedPostcode.IsNullOrEmpty() &&
                !nm.Customer.Surname.IsNullOrEmpty()).ToList())
            {
                sapMandatoryFields.Add(new SapByMandatoryFields
                {
                    TradingName = Customer.Surname,
                    Address1 = FinalAddress,
                    Postcode = NormalizedPostcode.RepresentUKPostcode(),
                    Key = i.ToString()
                });

                i++;
            }

            var buildersSap = new List<BuilderModel>();

            if (sapMandatoryFields.Any())
            {
                var sapMandatoryResponse =
                    sapService.GetSapBuilderAsync(sapMandatoryFields, 1).GetAwaiter().GetResult();

                if (sapMandatoryResponse.HasError())
                {
                    logger.LogError("Data Migration GetSapEntities Error");

                    return new List<BuilderModel>();
                }

                if (sapMandatoryResponse.Content != null)
                {
                    buildersSap.AddRange(sapMandatoryResponse.Content.Values.SelectMany(b => b));
                }
            }

            if (accountNumbers.Any())
            {
                var sapAccountResponse = sapService.GetSapBuilder(accountNumbers).GetAwaiter().GetResult();

                if (sapAccountResponse.HasError())
                {
                    logger.LogError("Data Migration GetSapEntities Error");

                    return new List<BuilderModel>();
                }

                if (sapAccountResponse.Content != null)
                {
                    buildersSap.AddRange(sapAccountResponse.Content);
                }
            }

            return buildersSap;
        }

        /// <summary>
        ///     Map relationships with other tables
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void MapRelationships(CustomerMigrationModel model, Builder entity)
        {
            var AiepId = Convert.ToInt32(model.AiepTdpId);

            if (entity.Id > 0) // Found on DB
            {
                if (!entity.BuilderEducationerAieps.Any(x => x.BuilderId == entity.Id && x.AiepId == AiepId) &&
                    !DbSet<BuilderEducationerAiep>().Any(x => x.BuilderId == entity.Id && x.AiepId == AiepId))
                {
                    var bdp = new BuilderEducationerAiep { BuilderId = entity.Id, AiepId = AiepId };
                    entity.BuilderEducationerAieps.Add(bdp);
                    Add(bdp);
                }
            }
            else // Found on Memory or Brand new
            {
                if (!entity.BuilderEducationerAieps.Any(x => x.AiepId == AiepId))
                {
                    var bdp = new BuilderEducationerAiep { Builder = entity, AiepId = AiepId };
                    entity.BuilderEducationerAieps.Add(bdp);
                    Add(bdp);
                }
            }
        }

        private List<(string NormalizedPostcode, string FinalAddress, CustomerMigrationModel Customer)> NormalizeModels(
            IEnumerable<CustomerMigrationModel> models)
        {
            var modelsNormalized = models.Select(m =>
            {
                return (m.Postcode.NormalisePostcode(), GetFinalAddress(m), m);
            }).ToList();

            return modelsNormalized;
        }

        #endregion

    }

}

