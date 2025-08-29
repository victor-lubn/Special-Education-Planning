using Koa.Domain.Search.Page;
using Koa.Persistence.Abstractions.QueryResult;
using Koa.Persistence.EntityRepository.EntityFrameworkCore.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface IRegionRepository : IBaseRepository<Region>
    {
        Task<RepositoryResponse<ICollection<AreaModel>>> GetRegionAreas(int regionId);
        Task<RepositoryResponse<IPagedQueryResult<RegionModel>>> GetRegionsFilteredAsync(IPageDescriptor searchModel);
        Task<RepositoryResponse<RegionModel>> GetDuplicatedRegion(RegionModel regionModel);
    }
}