using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Enum;
using Action = SpecialEducationPlanning
.Domain.Entity.Action;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IActionRepository :IBaseRepository<Action>
    {
        Task<RepositoryResponse<ICollection<ActionModel>>> GetModelActions<T>(int id);
        Task<RepositoryResponse<ICollection<ActionModel>>> GetPlanActions<TPlan, TVersion>(int id);
        Task CreateAction<T>(ActionType actionTypeEnum, string additionalInfo, int modelId, string userInfo);
    }
}
