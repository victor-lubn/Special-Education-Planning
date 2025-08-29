using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IAreaRepository : IBaseRepository<Area>
    {
        Task<RepositoryResponse<ICollection<AiepModel>>> GetAreaAieps(int areaId);
        Task<RepositoryResponse<IPagedQueryResult<AreaModel>>> GetAreasFilteredAsync(IPageDescriptor searchModel);
        Task<RepositoryResponse<AreaModel>> SaveArea(AreaDtoModel areaDtoModel);
    }
}
