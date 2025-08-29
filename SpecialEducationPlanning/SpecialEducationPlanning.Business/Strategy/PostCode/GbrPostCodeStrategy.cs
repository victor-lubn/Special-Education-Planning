using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using SpecialEducationPlanning
.Business.Model.PostCode;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Business.Strategy.PostCode;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Strategy.PostCode
{
    public class GbrPostCodeStrategy : IPostCodeStrategy
    {
        // Official UK Gov regex with space in between
        private const string POSTCODE_REGEX = "(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]))))\\s?[0-9][A-Z-[CIKMOV]]{2})";
        private const string NO_POSTCODE_CODE = "N/P";
        protected readonly ILogger<GbrPostCodeStrategy> logger;

        public GbrPostCodeStrategy(ILogger<GbrPostCodeStrategy> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Setting spacing in between postcode
        /// Example: postcode like w1D1Nn to w1 D1Mm.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        private string FormatPostcode(string postcode)
        {
            logger.LogDebug("GbrPostCodeStrategy called FormatPostcode");

            switch (postcode.Length)
            {
                //add space after 2 characters if length is 5 
                case 5: postcode = postcode.Insert(2, " "); break;
                //add space after 3 characters if length is 6 
                case 6: postcode = postcode.Insert(3, " "); break;
                //add space after 4 characters if length is 7 
                case 7: postcode = postcode.Insert(4, " "); break;

                default: break;
            }

            logger.LogDebug("GbrPostCodeStrategy end call -> return String");

            return postcode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public bool IsValidPostcode(string postcode)
        {
            logger.LogDebug("GbrPostCodeStrategy called IsValidPostcode");
            if (postcode.IsNullOrEmpty())
                return false;

            postcode = NormalisePostcode(postcode);

            if (postcode.Equals(NO_POSTCODE_CODE))
                return true;

            if (postcode.Length < 5 || postcode.Length > 7)
                return false;

            var match = Regex.Match(postcode, POSTCODE_REGEX);

            logger.LogDebug("GbrPostCodeStrategy end call IsValidPostcode -> return Bool");

            return match.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string RepresentPostcode(string postcode)
        {
            postcode = NormalisePostcode(postcode);
            postcode = FormatPostcode(postcode);

            logger.LogDebug("GbrPostCodeStrategy end call RepresentPostcode");

            return postcode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string NormalisePostcode(string postcode)
        {
            logger.LogDebug("GbrPostCodeStrategy end call NormalisePostcode -> return String");

            return postcode.Trim().Replace(" ", string.Empty).ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public RepositoryResponse<string> GetPostCode(string postcode)
        {
            logger.LogDebug("GbrPostCodeStrategy called GetPostCode");

            var repositoryResponse = new RepositoryResponse<string>();
            var postCode = this.RepresentPostcode(postcode);
            if (!this.IsValidPostcode(postCode))
            {
                logger.LogDebug("PostCode format error");

                logger.LogDebug("GbrPostCodeStrategy end call GetPostCode -> return Entity not found");

                return new RepositoryResponse<string>
                {
                    Content = null,
                    ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() }
                };
            }
            repositoryResponse.Content = this.NormalisePostcode(postCode);

            logger.LogDebug("GbrPostCodeStrategy end call GetPostCode -> return Repository response String");

            return repositoryResponse;
        }
    }
}
