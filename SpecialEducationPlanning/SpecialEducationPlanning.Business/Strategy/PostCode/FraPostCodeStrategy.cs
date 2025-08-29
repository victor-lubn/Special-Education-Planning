using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Strategy.PostCode
{
    public class FraPostCodeStrategy : IPostCodeStrategy
    {
        private const string NO_POSTCODE_CODE = "N/P";
        protected readonly ILogger<FraPostCodeStrategy> logger;
        public FraPostCodeStrategy(ILogger<FraPostCodeStrategy> logger)
        {
            this.logger = logger;
        }

        public RepositoryResponse<string> GetPostCode(string postcode)
        {
            logger.LogDebug("FraPostCodeStrategy called GetPostCode");

            var repositoryResponse = new RepositoryResponse<string>();
            if (!this.IsValidPostcode(postcode))
            {
                logger.LogDebug("PostCode format error");

                logger.LogDebug("FraPostCodeStrategy end call GetPostCode -> return Entity not found");

                return new RepositoryResponse<string>
                {
                    Content = null,
                    ErrorList = new Collection<string> { ErrorCode.EntityNotFound.GetDescription() }
                };
            }
            repositoryResponse.Content = NormalisePostcode(postcode);

            logger.LogDebug("FraPostCodeStrategy end call GetPostCode -> return Repository response String");

            return repositoryResponse;
        }

        /// <summary>
        /// Validates if the string consists of 5 digits.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public bool IsValidPostcode(string postcode)
        {
            logger.LogDebug("FraPostCodeStrategy called IsValidPostcode");
            if (postcode.IsNullOrEmpty())
                return false;

            postcode = NormalisePostcode(postcode);

            if (postcode.Equals(NO_POSTCODE_CODE)) 
                return true;

            if (postcode.Length != 4 && postcode.Length != 5)
                return false;
            logger.LogDebug("FraPostCodeStrategy end call IsValidPostcode -> return Bool");

            return postcode.All(char.IsDigit);
        }

        /// <summary>
        /// Removes whitespaces
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string NormalisePostcode(string postcode)
        {
            logger.LogDebug("FraPostCodeStrategy end call NormalisePostcode -> return String");

            return postcode.Trim().Replace(" ", String.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public string RepresentPostcode(string postcode)
        {
            postcode = NormalisePostcode(postcode);

            logger.LogDebug("FraPostCodeStrategy end call RepresentPostcode");

            return postcode;
        }
    }
}
