using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Repository
{
    public interface ICatalogRepository : IBaseRepository<Catalog>
    {
        Task<RepositoryResponse<CatalogModel>> GetCodeFromCatalogue(string catalog, string EducationOrigin);
        Task<RepositoryResponse<CatalogModel>> GetCatalogueFromCode(string code, string EducationOrigin);
        Task<RepositoryResponse<IEnumerable<CatalogModel>>> GetCatalogs(string EducationOrigin);
        Task<RepositoryResponse<CatalogModel>> GetCatalogById(int catalogId);
        Task<RepositoryResponse<CatalogModel>> CreateCatalog(CatalogModel value);
    }
}

