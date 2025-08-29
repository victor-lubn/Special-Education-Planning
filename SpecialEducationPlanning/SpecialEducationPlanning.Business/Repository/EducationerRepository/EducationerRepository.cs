using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Repository
{

    public class EducationerRepository : BaseRepository<User>, IEducationerRepository
    {
        private readonly ILogger<EducationerRepository> logger;

        public EducationerRepository(ILogger<EducationerRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
            base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.logger = logger;
        }

        #region Implements IEducationerRepository

        public async Task<RepositoryResponse<int?>> GetPendingReleaseInfo(int EducationerId)
        {
            logger.LogDebug("EducationerRepository called GetPendingReleaseInfo");

            var Educationer = await base.FindOneAsync<User>(EducationerId);

            if (Educationer?.AiepId == null)
            {
                logger.LogDebug("EducationerRepository end call GetPendingReleaseInfo -> return Repository response Errors Entity not found Educationer");

                return new RepositoryResponse<int?>(null, ErrorCode.EntityNotFound);
            }

            var Aiep = await base.FindOneAsync<Aiep>(Educationer.AiepId.Value);

            if (Aiep == null)
            {
                logger.LogDebug("EducationerRepository end call GetPendingReleaseInfo -> return Repository response Errors Entity not found Aiep");

                return new RepositoryResponse<int?>(null, ErrorCode.EntityNotFound, "AiepId not found");
            }

            logger.LogDebug("EducationerRepository end call GetPendingReleaseInfo -> return Repository response Int");

            return Educationer.ReleaseInfoId != Aiep.ReleaseInfoId
                ? new RepositoryResponse<int?>(Aiep.ReleaseInfoId)
                : new RepositoryResponse<int?>(content: null);
        }

        #endregion

    }

}

