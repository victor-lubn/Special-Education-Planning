using Koa.Domain.Specification;
using Koa.Domain.Specification.Search;
using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification.UserReleaseInfoSpecifications;

namespace SpecialEducationPlanning
.Business.Repository.UserReleaseInfoRepository
{
    public class UserReleaseInfoRepository : BaseRepository<UserReleaseInfo>, IUserReleaseInfoRepository
    {
        private readonly IReleaseInfoRepository releaseInfoRepository;
        private readonly ILogger<UserReleaseInfoRepository> logger;
        private readonly IObjectMapper mapper;

        public UserReleaseInfoRepository(ILogger<UserReleaseInfoRepository> logger, IEntityRepository<int> repositoryKey, IEfUnitOfWork unitOfWork, IObjectMapper mapper,
            IDbContextAccessor dbContextAccessor, IReleaseInfoRepository releaseInfoRepository, ISpecificationBuilder specificationBuilder, IEntityRepository repository) :
base(logger, repository, unitOfWork, specificationBuilder, repositoryKey, dbContextAccessor)
        {
            this.releaseInfoRepository = releaseInfoRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> ExistsUserReleaseInfo(int releaseInfoId, int userId)
        {
            logger.LogDebug("UserReleaseInfoRepository called ExistsUserReleaseInfo");

            var spec = Specification<UserReleaseInfo>.True;
            spec = spec.And(new UserReleaseInfoByUserSpecification(userId));
            spec = spec.And(new UserReleaseInfoByReleaseInfoIdSpecification(releaseInfoId));
            var userReleaseInfo = await repository.Where(spec).FirstOrDefaultAsync();

            logger.LogDebug("UserReleaseInfoRepository end call ExistsUserReleaseInfo -> return userReleaseInfo.IsNotNull");

            return userReleaseInfo.IsNotNull() ? true : false;
        }

        public async Task<RepositoryResponse<UserReleaseInfoModel>> CreateUserReleaseInfoAsync(int releaseInfoId, int userId)
        {
            logger.LogDebug("UserReleaseInfoRepository called CreateUserReleaseInfoAsync");

            var response = new RepositoryResponse<UserReleaseInfoModel>();

            UnitOfWork.BeginTransaction();
            var userReleaseInfo = new UserReleaseInfo
            {
                ReleaseInfoId = releaseInfoId,
                UserId = userId
            };

            repository.Add(userReleaseInfo);

            logger.LogDebug("UserReleaseInfoRepository CreateUserReleaseInfoAsync call Commit");

            UnitOfWork.Commit();

            response.Content = mapper.Map<UserReleaseInfo, UserReleaseInfoModel>(userReleaseInfo);

            logger.LogDebug("UserReleaseInfoRepository end call CreateUserReleaseInfoAsync -> return Repository response UserReleaseInfoModel");

            return response;
        }

        public async Task<bool> DeleteUserReleaseInfoAsync(string version, string fusionVersion)
        {
            logger.LogDebug("UserReleaseInfoRepository called DeleteUserReleaseInfoAsync");

            try
            {
                var response = new RepositoryResponse<UserReleaseInfoModel>();

                var releaseInfo = await releaseInfoRepository.GetReleaseInfoAsync(version, fusionVersion);

                if (releaseInfo.Content.IsNotNull())
                {
                    UnitOfWork.BeginTransaction();
                    var spec = new UserReleaseInfoByReleaseInfoIdSpecification(releaseInfo.Content.Id);
                    var userReleasesInfo = await repository.Where(spec).ToListAsync();

                    foreach (var userReleaseInfo in userReleasesInfo)
                    {
                        base.Remove(userReleaseInfo.Id);
                    }

                    logger.LogDebug("UserReleaseInfoRepository DeleteUserReleaseInfoAsync call Commit");

                    UnitOfWork.Commit();
                }

                logger.LogDebug("UserReleaseInfoRepository end call DeletetUserReleaseInfoAsync -> return True");

                return true;
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();

                logger.LogDebug("UserReleaseInfoRepository end call DeleteUserReleaseInfoAsync -> return False");

                return false;
            }

        }
    }
}
