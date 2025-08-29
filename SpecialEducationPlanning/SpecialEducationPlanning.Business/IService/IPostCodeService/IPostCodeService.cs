using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Business.Model.PostCode;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Business.IService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPostCodeService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        string RepresentPostcode(string postcode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        string NormalisePostcode(string postcode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        bool IsValidPostcode(string postcode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        RepositoryResponse<string> GetPostCode(string postcode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        Task<RepositoryResponse<IEnumerable<AddressSearchResultModel>>> SearchAddressAsync(string postcode);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<RepositoryResponse<AddressModel>> GetAddressByUriAsync(string uri);
    }
}
