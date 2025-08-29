using Koa.Domain.Search.Page;
using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Query;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Specification.ReleaseInfoSpecifications;

namespace SpecialEducationPlanning
.Business.Repository.ReleaseNoteRepository
{
    public class ReleaseInfoRepository : BaseRepository<ReleaseInfo>, IReleaseInfoRepository
    {
        private readonly ILogger<ReleaseInfoRepository> logger;

        private readonly IObjectMapper mapper;

        public ReleaseInfoRepository(ILogger<ReleaseInfoRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) : base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor) {
            this.mapper = mapper;
            this.logger = logger;
        }


        public async Task<RepositoryResponse<ReleaseInfoModel>> SetReleaseInfoDocument(int id, string path, string fileName)
        {
            logger.LogDebug("ReleaseInfoRepository called SetReleaseInfoDocument");

            var entity = await base.FindOneAsync<ReleaseInfo>(id);
            if (entity.IsNull())
            {
                logger.LogDebug("ReleaseInfoRepository end call SetReleaseInfoDocument -> return Repository response Errors Entity not found");

                return new RepositoryResponse<ReleaseInfoModel>(ErrorCode.EntityNotFound.GetDescription());
            }

            entity.DocumentPath = path;
            entity.Document = fileName;

            logger.LogDebug("ReleaseInfoRepository SetReleaseInfoDocument call Commit");

            UnitOfWork.Commit();

            logger.LogDebug("ReleaseInfoRepository end call SetReleaseInfoDocument -> return Repository response ReleaseInfoModel");

            return new RepositoryResponse<ReleaseInfoModel>(mapper.Map<ReleaseInfo, ReleaseInfoModel>(entity));
        }

        public async Task<RepositoryResponse<ReleaseInfoModel>> GetNewestReleaseInfoDocumentAsync()
        {
            logger.LogDebug("ReleaseInfoRepository called GetNewestReleaseInfoDocumentAsync");

            var spec = new Specification<ReleaseInfo>(x => x.DateTime.IsNotNull());
            var result = await repository.Where(spec).OrderByDescending(x => x.DateTime).FirstOrDefaultAsync();

            logger.LogDebug("ReleaseInfoRepository end call GetNewestReleaseInfoDocumentAsync -> return Repository response ReleaseInfoModel");

            return new RepositoryResponse<ReleaseInfoModel>(mapper.Map<ReleaseInfo, ReleaseInfoModel>(result));
        }

        public async Task<RepositoryResponse<ReleaseInfoModel>> GetReleaseInfoAsync(string version, string fusionVersion)
        {
            logger.LogDebug("ReleaseInfoRepository called GetReleaseInfoAsync");

            var response = new RepositoryResponse<ReleaseInfoModel>();
            ReleaseInfo releaseInfo;

            var specVersions = new ReleaseInfoVersionsSpecification(version, fusionVersion);
            releaseInfo = await repository.Where(specVersions).FirstOrDefaultAsync();

            if (releaseInfo.IsNotNull())
            {
                response.Content = mapper.Map<ReleaseInfo, ReleaseInfoModel>(releaseInfo);

                logger.LogDebug("ReleaseInfoRepository end call GetReleaseInfoAsync -> return Repository response ReleaseInfoModel");

                return response;
            }
            response.Content = null;

            logger.LogDebug("ReleaseInfoRepository end call GetReleaseInfoAsync -> return Repository response ReleaseInfoModel Null content");

            return response;
        }

        public async Task<RepositoryResponse<IPagedQueryResult<ReleaseInfoModel>>> GetReleaseInfoFilteredAsync(IPageDescriptor searchModel)
        {
            logger.LogDebug("ReleaseInfoRepository called GetReleaseInfoFilteredAsync");

            ISpecification<ReleaseInfo> spec = Specification<ReleaseInfo>.True;

            var modelSpec = SpecificationBuilder.Create<ReleaseInfoModel>(searchModel.Filters);

            var query = new ReleaseInfoMaterializedReleaseInfoModelPagedValueQuery(spec, modelSpec, searchModel.Sorts, searchModel);
            var result = repository.Query(query);

            logger.LogDebug("ReleaseInfoRepository end call GetReleaseInfoFilteredAsync -> return Repository response Paged query ReleaseInfoModel");

            return new RepositoryResponse<IPagedQueryResult<ReleaseInfoModel>>(result);
        }
    }
}
