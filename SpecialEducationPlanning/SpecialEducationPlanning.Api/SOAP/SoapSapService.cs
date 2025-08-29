using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.SapConfiguration;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.SOAP
{
    /// <summary>
    /// Service to interact with SAP
    /// </summary>
    public class SoapSapService : ISoapSapService
    {
        private readonly IBuilderRepository builderRepository;
        private readonly ILogger<SoapSapService> logger;
        private readonly IOptions<SapConfiguration> sapConfiguration;
        public readonly static string UrlPath = "/SoapSapService";

        /// <summary>
        /// </summary>
        /// <param name="builderRepository"></param>
        public SoapSapService(IBuilderRepository builderRepository, ILogger<SoapSapService> logger, IOptions<SapConfiguration> sapConfiguration)
        {
            this.builderRepository = builderRepository;
            this.logger = logger;
            this.sapConfiguration = sapConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builderSapModels"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSapBuilders(List<BuilderSapModel> builderSapModels)
        {
            try
            {
                if (builderSapModels.Any(b => string.IsNullOrEmpty(b.TradingName)))
                {
                    logger.LogDebug("Error in UpdateSapBuilders due to empty trading name - SoapSapService");
                    return false;
                }

                if (!builderSapModels.Any(b => sapConfiguration.Value.Companies.Contains(b.Company)))
                {
                    logger.LogDebug("Error in UpdateSapBuilders due to Company values not matching - SoapSapService");
                    return false;
                }

                await builderRepository.UpdateBuildersFromSapAsync(builderSapModels);
                return true;
            }
            catch (Exception)
            {
                logger.LogError("Error in UpdateSapBuilders - SoapSapService");
                return false;
            }
        }
    }
}
