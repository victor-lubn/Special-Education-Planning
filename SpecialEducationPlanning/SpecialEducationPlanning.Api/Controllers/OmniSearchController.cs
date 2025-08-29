using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.OmniSearchModel;
using SpecialEducationPlanning
.Api.Service.OmniSearch;
using SpecialEducationPlanning
.Api.Service.Search;
using SpecialEducationPlanning
.Api.Service.User;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Controllers
{

    /// <summary>
    ///     Omni Search Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OmniSearchController : Controller
    {

        private readonly IAzureSearchService azureSearchService;

        private readonly IBuilderRepository builderRepository;

        private readonly IAiepRepository AiepRepository;

        private readonly IOmniSearchService omniSearchService;

        private readonly IPlanRepository planRepository;
        
        private readonly IProjectRepository projectRepository;

        private readonly IUserService userService;

        private readonly OmniSearchConfiguration omniSearchConfiguration;

        private readonly ILogger<OmniSearchController> logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="builderRepository"></param>
        /// <param name="AiepRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="omniSearchService"></param>
        /// <param name="userService"></param>
        public OmniSearchController(IBuilderRepository builderRepository, IAiepRepository AiepRepository,
            IPlanRepository planRepository, IProjectRepository projectRepository, IOmniSearchService omniSearchService, IUserService userService,
            IAzureSearchService azureSearchService, IOptions<OmniSearchConfiguration> omniSearchConfiguration,
            ILogger<OmniSearchController> logger)
        {
            this.builderRepository = builderRepository;
            this.AiepRepository = AiepRepository;
            this.planRepository = planRepository;
            this.projectRepository = projectRepository;
            this.omniSearchService = omniSearchService;
            this.userService = userService;
            this.azureSearchService = azureSearchService;
            this.omniSearchConfiguration = omniSearchConfiguration.Value;

            this.logger = logger;
        }

        #region Methods Public

        /// <summary>
        ///     Searches for Builder and Plan models
        /// </summary>
        /// <returns>Returns Builder and Plan models. This action is not returning archived plans</returns>
        [HttpPost]
        public async Task<IActionResult> PostOmniSearchResult([FromBody] OmniSearchRequestModel omniSearchRequestModel)
        {
            logger.LogDebug("OmniSearchController called PostOmniSearchResult");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("OmniSearchController end call PostOmniSearchResult -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            if (!omniSearchRequestModel.TextToSearch.IsNullOrEmpty() && omniSearchRequestModel.TextToSearch.Length < 3)
            {
                logger.LogDebug("OmniSearchController end call PostOmniSearchResult -> return Bad request short text");

                return new BadRequestObjectResult("TextToSearch Length < 3");
            }

            if (omniSearchRequestModel.TextToSearch.IsNullOrEmpty())
            {
                omniSearchRequestModel.PageSize = omniSearchConfiguration.MaxResultsEmptySearch;
            }
            else if (omniSearchRequestModel.PageSize > omniSearchConfiguration.MaxResults)
            {
                omniSearchRequestModel.PageSize = omniSearchConfiguration.MaxResults;
            }

            if (omniSearchRequestModel.PageSize < 0)
            {
                logger.LogDebug("OmniSearchController end call PostOmniSearchResult -> return Bad request Page size < 0");

                return new BadRequestObjectResult("PageSize < 0");
            }

            if (omniSearchRequestModel.PageNumber < 1)
            {
                logger.LogDebug("OmniSearchController end call PostOmniSearchResult -> return Bad request Page number < 1");

                return new BadRequestObjectResult("PageNumber < 1");
            }


            //var skip = (omniSearchRequestModel.PageNumber - 1) * omniSearchRequestModel.PageSize;
            var skip = 0;

            logger.LogDebug("OmniSearchController end call PostOmniSearchResult -> return Call AzureSearch");

            return await AzureSearch(omniSearchRequestModel.TextToSearch, skip);
        }

        #endregion

        #region Methods Private

        private async Task<IActionResult> AzureSearch(string textToSearch, int skip = 0)
        {
            logger.LogDebug("OmniSearchController called AzureSearch");

            int takeValue = omniSearchConfiguration.TakeEntries;

            logger.LogDebug("OmniSearchController AzureSearch takeValue -> {takeValue}", takeValue.ToString());

            var currentAiepId = userService.GetUserCurrentAiepId((ClaimsIdentity)User.Identity);
            var entityTypesAndIds = await azureSearchService.OmniSearchSearchAsync(textToSearch, currentAiepId, skip, takeValue);

            var builders = await builderRepository.GetBuildersByIdsAsync(entityTypesAndIds[typeof(Domain.Entity.Builder)], skip, takeValue, currentAiepId, null);
            var plans = await planRepository.GetPlansByIdsAndTypeAsync(entityTypesAndIds, skip, takeValue, currentAiepId, null, null);
            var projects = await projectRepository.GetProjectsByIdsAndTypeAsync(entityTypesAndIds, skip, takeValue, currentAiepId, null, null);

            var omniSearchModels = omniSearchService.ListResults(builders.Content, plans.Content, projects.Content).Take(takeValue);

            var omniSearchResponse = new OmniSearchResponse
            {
                OmniSearchItemsList = omniSearchModels,
                totalCount = omniSearchModels.Count(),
                MaxExceeded = false
            };

            logger.LogDebug("OmniSearchController end call AzureSearch -> return Ok oject");

            return new OkObjectResult(omniSearchResponse);
        }

        #endregion

    }

}
