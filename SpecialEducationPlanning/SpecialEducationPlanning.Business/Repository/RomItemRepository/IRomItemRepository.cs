using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IRomItemRepository : IBaseRepository<RomItem>
    {
        Task<RepositoryResponse<RomItem>> GetRomItemByNameAsync(string name);
        Task<RepositoryResponse<bool>> DeleteRomItemsFromVersion(int versionId);
        Task<RepositoryResponse<RomItemModel>> CreateRomItem(RomItemModel romItemModel, int versionId, int catalogId);
    }
}
