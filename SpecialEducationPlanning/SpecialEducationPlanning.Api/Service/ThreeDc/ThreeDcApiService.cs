using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpecialEducationPlanning
.Api.Configuration.ThreeDcApi;
using SpecialEducationPlanning
.Api.Model.FittersPackModel;
using SpecialEducationPlanning
.Api.Service.ApimToken;
using SpecialEducationPlanning
.Api.Service.Base;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.ThreeDc
{
    public class ThreeDcApiService : ApimServiceBase, IThreeDcApiService
    {
        public ThreeDcApiService(
            IOptions<ThreeDcApiServiceConfiguration> options,
            ILogger<ThreeDcApiService> logger,
            HttpClient httpClient,
            IGenerateApimTokenService apimTokenService)
            : base(options.Value, logger, httpClient, apimTokenService)
        {
        }

        public async Task<RepositoryResponse<GenerateFittersPackResponseModel>> GenerateFitterPack(
            GenerateFittersPackRequestModel model)
        {
            var result = await this.Post<GenerateFittersPackResponseModel>("/generateFitterPack", model);
            return result;
        }
    }
}
