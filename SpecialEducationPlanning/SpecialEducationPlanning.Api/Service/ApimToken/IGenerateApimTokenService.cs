using System;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.Base;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.ApimToken
{
    public interface IGenerateApimTokenService
    {
        public Task<RepositoryResponse<string>> GenerateApimTokenAsync(ApimConfigurationBase apimConfiguration);
    }
}
