using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IEducationerRepository : IBaseRepository<User>
    {

        Task<RepositoryResponse<int?>> GetPendingReleaseInfo(int EducationerId);

    }
}
