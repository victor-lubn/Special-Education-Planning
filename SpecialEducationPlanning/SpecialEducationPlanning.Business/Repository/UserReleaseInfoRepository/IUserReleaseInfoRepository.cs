
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository.UserReleaseInfoRepository
{
    public interface IUserReleaseInfoRepository : IBaseRepository<UserReleaseInfo>
    {
        Task<bool> ExistsUserReleaseInfo(int releaseInfoId, int userId);
        Task<RepositoryResponse<UserReleaseInfoModel>> CreateUserReleaseInfoAsync(int releaseInfoId, int userId);
        Task<bool> DeleteUserReleaseInfoAsync(string version, string fusionVersion);
    }
}
