using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SpecialEducationPlanning
.Business.Model.PostCode;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Strategy.PostCode
{
    class IrlPostCodeStrategy : IPostCodeStrategy
    {
        private const string NO_POSTCODE_CODE = "N/P";
        protected readonly ILogger<IrlPostCodeStrategy> logger;
        public IrlPostCodeStrategy(ILogger<IrlPostCodeStrategy> logger)
        {
            this.logger = logger;
        }


        /// <summary>
        /// Setting spacing in between postcode
        /// Example: postcode like D02AF30 to D02 AF30.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        private string FormatPostcode(string postcode)
        {
            logger.LogDebug("IrlPostCodeStrategy called FormatPostcode");
            //add space after 3 characters
            return postcode.Insert(3, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public bool IsValidPostcode(string postcode)
        {
            logger.LogDebug("IrlPostCodeStrategy called IsValidPostcode");
            if (postcode.IsNullOrEmpty())
                return false;

            postcode = NormalisePostcode(postcode);

            if (postcode.Equals(NO_POSTCODE_CODE))
                return true;

            if (postcode.Length == 7)
                return true;

            return false;
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

            logger.LogDebug("IrlPostCodeStrategy end call RepresentPostcode");

            return postcode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string NormalisePostcode(string postcode)
        {
            logger.LogDebug("IrlPostCodeStrategy end call NormalisePostcode -> return String");

            return postcode.Trim().Replace(" ", string.Empty).ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public RepositoryResponse<string> GetPostCode(string postcode)
        {
            logger.LogDebug("IrlPostCodeStrategy called GetPostCode");

            var repositoryResponse = new RepositoryResponse<string>();
            var postCode = this.RepresentPostcode(postcode);
            if (!this.IsValidPostcode(postCode))
            {
                logger.LogDebug("PostCode format error");

                logger.LogDebug("IrlPostCodeStrategy end call GetPostCode -> return Entity not found");

                return new RepositoryResponse<string>
                {
                    Content = null,
                    ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() }
                };
            }
            repositoryResponse.Content = this.NormalisePostcode(postCode);

            logger.LogDebug("IrlPostCodeStrategy end call GetPostCode -> return Repository response String");

            return repositoryResponse;
        }
    }
}
