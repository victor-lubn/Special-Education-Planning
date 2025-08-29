using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.BusinessCore;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Model.FileStorageModel;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Model.AzureSearchModel;
using SpecialEducationPlanning
.Domain.Service.Search;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.VersionSpecifications;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class VersionRepository : BaseRepository<Domain.Entity.Version>, IVersionRepository
    {
        private readonly IEntityRepository<int> entityRepositoryKey;
        private readonly IDbContextAccessor dbContextAccessor;
        private readonly ILogger<VersionRepository> logger;
        private readonly IFileStorageService<AzureStorageConfiguration> fileStorageService;
        private readonly IAzureSearchManagementService azureSearchManagementService;
        private readonly IObjectMapper mapper;
        private readonly IDbExecutionStrategy executionStrategy;

        public VersionRepository(ILogger<VersionRepository> logger, IEntityRepository<int> entityRepositoryKey, IEfUnitOfWork unitOfWork,
            IObjectMapper mapper, IDbContextAccessor dbContextAccessor, IFileStorageService<AzureStorageConfiguration> fileStorageService,
            IAzureSearchManagementService azureSearchManagementService, ISpecificationBuilder specificationBuilder, IDbExecutionStrategy executionStrategy, IEntityRepository entityRepository)
            : base(logger, entityRepository, unitOfWork, specificationBuilder, entityRepositoryKey, dbContextAccessor)
        {
            this.entityRepositoryKey = entityRepositoryKey;
            this.dbContextAccessor = dbContextAccessor;
            this.logger = logger;
            this.fileStorageService = fileStorageService;
            this.azureSearchManagementService = azureSearchManagementService;
            this.mapper = mapper;
            this.executionStrategy = executionStrategy;
        }

        public async Task<VersionModel> CreateVersionAsync(VersionModel value)
        {
            logger.LogDebug("VersionRepository called CreateVersionAsync");

            var sequenceVersionAiepCode = await entityRepositoryKey.GetNextExternalIdSequenceValueAsync(dbContextAccessor.GetCurrentContext(), value.PlanId, logger);
            value.Id = 0;
            value.ExternalCode = sequenceVersionAiepCode.Item1;
            value.AiepCode = sequenceVersionAiepCode.Item2;
            value.RomItems = new Collection<RomItemModel>();

            logger.LogDebug("VersionRepository end call CreateVersionAsync -> return Call ApplyChangesFixAsync");

            var versionEntity = mapper.Map<VersionModel,Domain.Entity.Version>(value);

            var applyChangesVersionEntity = await this.Add(versionEntity);

            var versionModel = mapper.Map<Domain.Entity.Version, VersionModel>(applyChangesVersionEntity);

            return versionModel;
        }


        public async Task<RepositoryResponse<VersionModel>> UpdateVersionAsync(Version value)
        {
            logger.LogDebug("VersionRepository called UpdateVersionAsync");

            logger.LogDebug("VersionRepository end call UpdateVersionAsync -> return Repository response Version Model Call AppylChangesFixAsync");

            var applyChangesVersionEntity = await ApplyChangesAsync(value);

            var versionModel = mapper.Map<Domain.Entity.Version, VersionModel>(applyChangesVersionEntity);

            return new RepositoryResponse<VersionModel>(versionModel);
        }

        public async Task<IEnumerable<VersionModel>> GetVersionsByPlanId(int planId)
        {
            logger.LogDebug("VersionRepository called GetVersionsByPlanId");

            var spec = new VersionsByPlanIdSpecification(planId);
            var versions = await this.repository.Where(spec)
                                                .Include(v => v.RomItems)
                                                .OrderByDescending(v => v.UpdatedDate)
                                                .ToListAsync();
            var response = mapper.Map<Domain.Entity.Version, VersionModel>(versions);

            logger.LogDebug("VersionRepository end call GetVersionByPlanId -> return List of VersionModel");

            return response;
        }

        public async Task<Version> GetVersionById(int versionId)
        {
            logger.LogDebug("VersionRepository called GetVersionById");

            var spec = new Specification<Version>(v => v.Id == versionId);
            var version = await this.repository.Where(spec)
                                                .Include(v => v.RomItems)
                                                .AsNoTracking()
                                                .SingleAsync();

            logger.LogDebug("VersionRepository end call GetVersionById -> return List of VersionModel");

            return version;
        }

        public async Task<RepositoryResponse<VersionModel>> SetVersionRom(int id, string path, string fileName, string versionNotes)
        {
            logger.LogDebug("VersionRepository called SetVersionRom");

            var entity = await base.FindOneAsync<Domain.Entity.Version>(id);
            if (entity.IsNotNull())
            {
                entity.VersionNotes = versionNotes;
                entity.RomPath = path;
                entity.Rom = fileName;

                logger.LogDebug("VersionRepository SetVersionRom call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("VersionRepository end call SetVersionRom -> return Repository response VersionModel");

                return new RepositoryResponse<VersionModel>(mapper.Map<Domain.Entity.Version, VersionModel>(entity));
            }

            logger.LogDebug("VersionRepository end call SetVersionRom -> return Repository response Errors Entity not found");

            return new RepositoryResponse<VersionModel>(ErrorCode.EntityNotFound.GetDescription());
        }

        public async Task<RepositoryResponse<VersionModel>> SetVersionPreview(int id, string path, string fileName)
        {
            logger.LogDebug("VersionRepository called SetVersionPreview");

            var entity = await base.FindOneAsync<Domain.Entity.Version>(id);
            if (entity.IsNotNull())
            {
                entity.PreviewPath = path;
                entity.Preview = fileName;

                logger.LogDebug("VersionRepository SetVersionPreview call Commit");

                UnitOfWork.Commit();

                logger.LogDebug("VersionRepository end call SetVersionPreview -> return Repository response VersionModel");

                return new RepositoryResponse<VersionModel>(mapper.Map<Domain.Entity.Version, VersionModel>(entity));
            }

            logger.LogDebug("VersionRepository end call SerVersionPreview -> return Repository response Errors Entity not found");

            return new RepositoryResponse<VersionModel>(ErrorCode.EntityNotFound.GetDescription());
        }

        public async Task<RepositoryResponse<Domain.Entity.Version>> GetRomAndPreviewInfo(int id)
        {
            logger.LogDebug("VersionRepository called GetRomAndPreviewInfo");

            var response = new Domain.Entity.Version();
            var version = await base.FindOneAsync<Domain.Entity.Version>(id);

            if (version.IsNull())
            {
                logger.LogDebug("VersionRepository end call GetRomAndPreviewInfo -> return Repository response Errors Entity not found");

                return new RepositoryResponse<Domain.Entity.Version>(ErrorCode.EntityNotFound.GetDescription());
            }

            response.Preview = version.Preview;
            response.PreviewPath = version.PreviewPath;
            response.Rom = version.Rom;
            response.RomPath = version.RomPath;

            logger.LogDebug("VersionRepository end call GetRomAndPreviewInfo -> return Repository response Version");

            return new RepositoryResponse<Domain.Entity.Version>(response);
        }

        public async Task<RepositoryResponse<string>> GetRomPathById(int id)
        {
            logger.LogDebug("VersionRepository called GetRomPathById");

            var path = (await base.FindOneAsync<Domain.Entity.Version>(id)).RomPath;

            logger.LogDebug("VersionRepository end call GetRomPathById -> return Repository response String Path");

            return new RepositoryResponse<string>()
            {
                Content = path
            };
        }

        public async Task<RepositoryResponse<int>> GetAvailableCatalogsAsync(int id)
        {
            logger.LogDebug("VersionRepository called GetAvailableCatalogsAsync");

            var spec = new Specification<Domain.Entity.Version>(x => x.Id == id);
            var version = await repository.Where(spec).Include(v => v.Plan).FirstOrDefaultAsync();

            logger.LogDebug("VersionRepository end call GetAvailableCatalogsAsync -> return Repository response Int");

            return new RepositoryResponse<int>(version.Plan.CatalogId);
        }

        public async Task<RepositoryResponse<VersionModel>> NewVersionAsync(int planId)
        {
            logger.LogDebug("VersionRepository called NewVersionAsync");

            var repositoryResponse = new RepositoryResponse<VersionModel>();
            //var version = await entityRepository.NewVersionAsync(dbContextAccessor.GetCurrentContext(), planId);

            //if (version.IsNull())
            //{
            //    repositoryResponse.ErrorList.Add(ErrorCode.EntityNotFound.GetDescription());
            //    repositoryResponse.ErrorList.Add(this.GetType() + " New Version Async");
            //}

            //repositoryResponse.Content = Mapper.Map<Version, VersionModel>(version);

            logger.LogDebug("VersionRepository end call NewVersionAsync -> return Repository response VersionModel");

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<VersionModel>> SaveVersion(int planId, int versionId, VersionInfoModel versionInfoModel)
        {
            logger.LogDebug("VersionRepository called SaveVersion");

            logger.LogDebug("Looking for {type}#{plan}", typeof(PlanModel), planId);
            var plan = await entityRepositoryKey.FindOneAsync<Plan>(planId);

            if (plan.IsNull())
            {
                logger.LogError("{type}#{plan} not found", typeof(PlanModel), planId);

                logger.LogDebug("VersionRepository end call SaveVersion -> return Repository response Errors Entity not found Plan");

                return new RepositoryResponse<VersionModel>(null, ErrorCode.EntityNotFound, "Plan not found");
            }

            try
            {
                var response = new RepositoryResponse<VersionModel>();

                await this.executionStrategy.ExecuteAsync(async () => 
                {
                    UnitOfWork.BeginTransaction();

                    var version = await entityRepositoryKey.CreateVersionAsync(dbContextAccessor.GetCurrentContext(), plan, versionId, versionInfoModel, logger);

                    if (version.IsNull())
                    {
                        logger.LogError("Error getting or creating version");
                        UnitOfWork.Rollback();

                        logger.LogDebug("VersionRepository end call SaveVersion -> return Repository response Errors Entity not found Plan");

                        response = new RepositoryResponse<VersionModel>(null, ErrorCode.EntityNotFound, "Version not found");
                    }

                    plan.Versions.Add(version);
                    logger.LogDebug("Version #{version} created for {type}#{plan}", version.Id, typeof(PlanModel), planId);

                    logger.LogDebug("VersionRepository SaveVersion call Commit");
                
                    UnitOfWork.Commit();

                    logger.LogDebug("VersionRepository end call SaveVersion -> return Repository response VersionModel");

                    response = new RepositoryResponse<VersionModel>(mapper.Map<Version, VersionModel>(version));
                }, CancellationToken.None);
                
                return response;
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("VersionRepository end call SaveVersion -> exception");
                logger.LogDebug("VersionRepository end call SaveVersion -> {message}", ex.Message);
                logger.LogDebug("VersionRepository end call SaveVersion -> {innerexception}", ex.InnerException);

                throw;
            }
        }

        public async Task<RepositoryResponse<VersionModel>> UploadRomFile(Stream streamFileModel, VersionModel versionModel)
        {
            logger.LogDebug("VersionRepository called UploadRomFile");

            var romRepositoryResponse = new RepositoryResponse<VersionModel>();
            //Uploads romFile            
            await fileStorageService.UploadOverrideVersionAsync(streamFileModel, versionModel);
            romRepositoryResponse = await SetVersionRom(versionModel.Id, versionModel.RomPath, "plan.rom", versionModel.VersionNotes);

            logger.LogDebug("VersionRepository end call UploadRomFile -> return Repository response VersionModel");

            return romRepositoryResponse;
        }

        public async Task<RepositoryResponse<VersionModel>> UploadRomFile(Stream romStream, string fileName, VersionModel versionModel)
        {
            logger.LogDebug("VersionRepository called UploadRomFile");

            var romRepositoryResponse = new RepositoryResponse<VersionModel>();
            //Uploads romFile            
            using (romStream)
            {
                var romPath = await fileStorageService.UploadOverrideVersionAsync(romStream, versionModel);
                romRepositoryResponse = await SetVersionRom(versionModel.Id, versionModel.RomPath, fileName, versionModel.VersionNotes);
            }

            logger.LogDebug("VersionRepository end call UploadRomFile -> return Repository response VersionModel");

            return romRepositoryResponse;
        }

        public async Task<RepositoryResponse<VersionModel>> UploadPreviewFile(Stream streamFileModel, int versionId)
        {
            logger.LogDebug("VersionRepository called UploadPreviewFile");

            //Uploads previewFile
            var previewPath = await fileStorageService.UploadAsync<Version>(streamFileModel);
            var previewRepositoryResponse = await SetVersionPreview(versionId, previewPath, "preview.jpeg");

            logger.LogDebug("VersionRepository end call UploadPreviewFile -> return Repository response VersionModel");

            return previewRepositoryResponse;
        }

        public async Task<RepositoryResponse<VersionModel>> UploadPreviewFile(Stream previewStream, string fileName, int versionId)
        {
            logger.LogDebug("VersionRepository called UploadPreviewFile");

            var previewRepositoryResponse = new RepositoryResponse<VersionModel>();
            //Uploads previewFile
            using (previewStream)
            {
                var previewPath = await fileStorageService.UploadAsync<Version>(previewStream);
                previewRepositoryResponse = await SetVersionPreview(versionId, previewPath, fileName);
            }

            logger.LogDebug("VersionRepository end call UploadPreviewFile -> return Repository response VersionModel");

            return previewRepositoryResponse;
        }

        public async Task<RepositoryResponse<VersionModel>> ModifyVersionNotes(int versionId, string versionNotes)
        {
            logger.LogDebug("VersionRepository called ModifyVersionNotes");

            var version = await base.FindOneAsync<Version>(versionId);

            if (version == null)
            {
                logger.LogDebug("Version Not Found");

                logger.LogDebug("VersionRepository end call ModifyVersionNotes -> return Repository response Errors Entity not found");

                return new RepositoryResponse<VersionModel>(null, ErrorCode.EntityNotFound, "Version Not Found");
            }

            this.executionStrategy.Execute(() =>
            {
                UnitOfWork.BeginTransaction();
                version.VersionNotes = versionNotes;

                logger.LogDebug("VersionRepository ModifyVersionNotes call Commit");

                UnitOfWork.Commit();
            });

            var versionModel = mapper.Map<Version, VersionModel>(version);

            logger.LogDebug("VersionRepository end call ModifyVersionNotes -> return Repository response VersionModel");

            return new RepositoryResponse<VersionModel>(versionModel);
        }

        public async Task<RepositoryResponse<VersionModel>> ModifyVersionNotesAndQuote(int versionId, string versionNotes, string quoteOrderNumber)
        {
            logger.LogDebug("VersionRepository called ModifyVersionNotesAndQuote");

            var version = await base.FindOneAsync<Version>(versionId);

            if (version == null)
            {
                logger.LogDebug("Version Not Found");

                logger.LogDebug("VersionRepository end call ModifyVersionNotesAndQuote -> return Repository response Errors Entity not found");

                return new RepositoryResponse<VersionModel>(null, ErrorCode.EntityNotFound, "Version Not Found");
            }

            this.executionStrategy.Execute(() =>
            {
                UnitOfWork.BeginTransaction();
                version.VersionNotes = versionNotes;
                version.QuoteOrderNumber = quoteOrderNumber;

                logger.LogDebug("VersionRepository ModifyVersionNotesAndQuote call Commit");

                UnitOfWork.Commit();
            });

            var versionModel = mapper.Map<Version, VersionModel>(version);

            logger.LogDebug("VersionRepository end call ModifyVersionNotesAndQuote -> return Repository response VersionModel");

            return new RepositoryResponse<VersionModel>(versionModel);
        }

        public async Task CallIndexerAsync(int take, int skip, DateTime? updateDate, int? indexerWindowInDays)
        {
            logger.LogDebug("VersionRepository called CallIndexerAsync");

            var versions = await base.Repository.GetEntitiesNoFiltersAsync<Version>(take, skip, updateDate, indexerWindowInDays).ToListAsync();

            azureSearchManagementService.MergeOrUploadDocuments
                (azureSearchManagementService.GetDocuments<OmniSearchVersionIndexModel, Version>(versions));

            logger.LogDebug("VersionRepository end call CallIndexerAsync");
        }

        public async Task<Version> GetVersionWithPlanProjectAiep(int versionId)
        {
            logger.LogDebug("VersionRepository called GetVersionWithPlanProjectAiep");

            logger.LogDebug("VersionRepository end call GetVersionWithPlanProjectAiep -> return Version");

            return await repository.Where(new EntityByIdSpecification<Version>(versionId))
                                    .Include(version => version.Plan)
                                    .ThenInclude(x => x.EducationToolOrigin)
                                    .Include(version => version.Plan)
                                    .ThenInclude(plan => plan.Project)
                                    .ThenInclude(project => project.Aiep)
                                    .FirstOrDefaultAsync();
        }
    }
}


