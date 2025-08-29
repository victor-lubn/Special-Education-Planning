using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     PostCode Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PostCodeController : Controller
    {
        
        private readonly IPostCodeServiceFactory postCodeServiceFactory;
        private readonly ILogger<PostCodeController> logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="postCodeService"></param>
        /// <param name="logger"></param>
        public PostCodeController(IPostCodeServiceFactory postCodeServiceFactory, ILogger<PostCodeController> logger)
        {
            this.postCodeServiceFactory = postCodeServiceFactory;
            this.logger = logger;
        }

        /// <summary>
        ///     Retrieve addresses from a specified postcode and country code
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        [HttpGet("Addresses")]
        public async Task<IActionResult> GetAddresses(string postcode, string countryCode)
        {
            logger.LogDebug("PostCodeController called GetAddresses");
            var postCodeService = postCodeServiceFactory.GetService(countryCode);
            if (!postCodeService.IsValidPostcode(postcode))
            {
                logger.LogDebug("PostCode format error");

                logger.LogDebug("PostCodeController end call GetAddresses -> return Bad request invalid PostCode");

                return new BadRequestObjectResult(ErrorCode.InvalidPostCode.GetDescription());
            }
            postcode = postCodeService.NormalisePostcode(postcode);

            var repositoryResponse =  await postCodeService.SearchAddressAsync(postcode);

            logger.LogDebug("PostCodeController end call GetAddresses -> return Call Search address");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get addresses from address line 1 and country code
        /// </summary>
        /// <param name="address1"></param>
        /// <returns></returns>
        [HttpGet("Addresses/Address1")]
        public async Task<IActionResult> GetAddressesByAddress1(string address1, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(address1))
            {
                logger.LogDebug("GetAddressesByAddress1 format error");
                logger.LogDebug("PostCodeController end call GetAddressesByAddress1 -> return Bad request invalid address1");
                return new BadRequestObjectResult(ErrorCode.InvalidAddress.GetDescription());
            }

            logger.LogDebug("PostCodeController called GetAddressesByAddress1");
            var postCodeService = postCodeServiceFactory.GetService(countryCode);
            var repositoryResponse = await postCodeService.SearchAddressAsync(address1);

            logger.LogDebug("PostCodeController end call GetAddressesByAddress1 -> return Call Search address");

            return repositoryResponse.GetHttpResponse();
        }


        /// <summary>
        /// Gets an indivdual, formatted address using a URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        [HttpGet("Addresses/Uri")]
        public async Task<IActionResult> GetAddressByUri(string uri, string countryCode)
        {
            logger.LogDebug("PostCodeController called GetAddressByUri");
            var postCodeService = postCodeServiceFactory.GetService(countryCode);
            var repositoryResponse = await postCodeService.GetAddressByUriAsync(uri);

            logger.LogDebug("PostCodeController end call GetAddressByUri -> return formatted address by URI");

            return repositoryResponse.GetHttpResponse();
        }
    }
}
