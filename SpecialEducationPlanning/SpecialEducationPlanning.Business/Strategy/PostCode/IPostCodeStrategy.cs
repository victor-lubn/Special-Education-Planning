
using SpecialEducationPlanning
.Business.Model.PostCode;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Business.Strategy.PostCode
{
    public interface IPostCodeStrategy
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
    }
}
