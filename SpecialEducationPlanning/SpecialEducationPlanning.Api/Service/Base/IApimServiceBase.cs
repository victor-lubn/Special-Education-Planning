using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.Base
{
    public interface IApimServiceBase
    {
        Task<RepositoryResponse<T>> Post<T>(string requestUri, object content);
    }
}
