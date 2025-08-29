using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Attributes;
using SpecialEducationPlanning
.Api.Configuration.Authorization;
using SpecialEducationPlanning
.Api.Configuration.ThreeDcApi;
using SpecialEducationPlanning
.Api.Constants;
using SpecialEducationPlanning
.Api.Controllers.Base;
using SpecialEducationPlanning
.Api.Model;
using SpecialEducationPlanning
.Business.Model.PlanDetails;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/api/[controller]")]
    public class PlanDetailsController : ControllerBaseExtended
    {
        private readonly IPlanRepository _planRepository;

        public PlanDetailsController(
            IPlanRepository planRepository,
            ILogger<PlanDetailsController> logger)
            : base(logger)
        {
            _planRepository = planRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiKeyAuthorize($"{ThreeDcApiRequestConfiguration.Section}:{nameof(ThreeDcApiRequestConfiguration.ApiKey)}")]
        [ProducesResponseType(typeof(PlanDetailsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status500InternalServerError)]
        [DefineHeader(CustomHeaders.aiepRequestConsumer, "API consumer application name", true)]
        [DefineHeader(CustomHeaders.aiepRequestIdentity, "API consumer identity name", false)]
        [DefineHeader(CustomHeaders.aiepApiKey, "API key", true)]
        public async Task<IActionResult> PlanDetails(
            [FromBody] PlanDetailsRequestModel planDetailsRequest)
        {
            _logger.LogDebug($"{nameof(PlanDetailsController)} called {nameof(PlanDetails)}");

            var requestCheck = ValidateRequest();
            if (requestCheck != null)
                return requestCheck;

            var repositoryResponse = await _planRepository.GetPlanDetailsAsync(planDetailsRequest);

            return ValidateResponse(repositoryResponse);
        }
    }
}
