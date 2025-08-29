using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IReleaseInfoRepository : IBaseRepository<ReleaseInfo>
    {
        Task<RepositoryResponse<ReleaseInfoModel>> SetReleaseInfoDocument(int id, string path, string fileName);
        Task<RepositoryResponse<ReleaseInfoModel>> GetNewestReleaseInfoDocumentAsync();
        Task<RepositoryResponse<ReleaseInfoModel>> GetReleaseInfoAsync(string version, string fusionVersion);
        Task<RepositoryResponse<IPagedQueryResult<ReleaseInfoModel>>> GetReleaseInfoFilteredAsync(IPageDescriptor searchModel);
    }
}
