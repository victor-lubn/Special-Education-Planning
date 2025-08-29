using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.FittersPackModel;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.ThreeDc
{
    public interface IThreeDcApiService
    {
        Task<RepositoryResponse<GenerateFittersPackResponseModel>> GenerateFitterPack(
            GenerateFittersPackRequestModel model);
    }
}
