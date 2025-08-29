using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Model.SapServiceModel;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Service.Sap
{

    /// <summary>
    /// </summary>
    public interface ISapService
    {

        #region Methods Public

        Task<RepositoryResponse<ValidationBuilderModel>> GetPosibleSapBuilder(BuilderModel builderModel);

        Task<RepositoryResponse<ValidationBuilderModel>> GetPossibleSapBuilderByAccountNumber(string accountNumber);

        /// <summary>
        ///     Get a mocked SAP builder
        /// </summary>
        /// <returns></returns>
        Task<RepositoryResponse<List<BuilderModel>>> GetSapBuilder(List<string> accountNumberList, int take = 1);

        Task<RepositoryResponse<List<BuilderModel>>> GetSapBuilderAsync(string surname, string postcode, string address);


        Task<RepositoryResponse<Dictionary<string, IEnumerable<BuilderModel>>>> GetSapBuilderAsync(IEnumerable<SapByMandatoryFields> sapByMandatoryFieldsEnumerable, int take = 20);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> HealthCheck();

        #endregion

    }

}