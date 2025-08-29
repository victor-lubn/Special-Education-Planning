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
    public interface ICountryRepository : IBaseRepository<Country>
    {
        Task<RepositoryResponse<ICollection<RegionModel>>> GetCountryRegions(int countryId);
        Task<RepositoryResponse<IPagedQueryResult<CountryModel>>> GetCountriesFilteredAsync(IPageDescriptor searchModel);
        Task<RepositoryResponse<CountryModel>> GetDuplicatedCountry(CountryModel countryModel);
    }
}