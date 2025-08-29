using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.DtoModel;

namespace SpecialEducationPlanning
.Api.SOAP
{
    /// <summary>
    /// Service to interact with SAP
    /// </summary>
    [ServiceContract]
    public interface ISoapSapService
    {
        /// <summary>
        /// Updates all the existing builders in TDP with the given ones
        /// </summary>
        /// <param name="builderModels"></param>
        /// <returns></returns>
        [OperationContract]
        Task<bool> UpdateSapBuilders(List<BuilderSapModel> builderModels);
    }
}
