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
    public interface ISortingFilteringRepository : IBaseRepository<SortingFiltering>
    {
        Task<RepositoryResponse<IEnumerable<SortingFilteringModel>>> GetSortingFilteringOptionsByEntity(string entity);
    }
}
