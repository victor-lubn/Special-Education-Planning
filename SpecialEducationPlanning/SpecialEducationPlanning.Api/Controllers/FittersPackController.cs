using Koa.Domain.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
.Business.Model.FittersPack;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using Version = SpecialEducationPlanning
.Domain.Entity.Version;

namespace SpecialEducationPlanning
.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/api/[controller]")]
    public class FittersPackController : ControllerBaseExtended
    {
        private readonly IVersionRepository _versionRepository;

        public FittersPackController(
            IVersionRepository planRepository,
            ILogger<FittersPackController> logger)
            : base(logger)
        {
            _versionRepository = planRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        [ApiKeyAuthorize($"{ThreeDcApiRequestConfiguration.Section}:{nameof(ThreeDcApiRequestConfiguration.ApiKey)}")]
        [ProducesResponseType(typeof(FittersPackResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetailsModel), StatusCodes.Status500InternalServerError)]
        [DefineHeader(CustomHeaders.aiepRequestConsumer, "API consumer application name", true)]
        [DefineHeader(CustomHeaders.aiepRequestIdentity, "API consumer identity name", false)]
        [DefineHeader(CustomHeaders.aiepApiKey, "API key", true)]
        public async Task<IActionResult> FittersPack(
            [FromBody] FittersPackRequestModel fittersPackRequestModel)
        {
            _logger.LogDebug($"{nameof(FittersPackController)} called {nameof(FittersPack)}");

            var requestCheck = ValidateRequest();
            if (requestCheck != null)
                return requestCheck;

            var version = (await _versionRepository
                    .WhereAsync(new Specification<Version>(p => 
                        p.FittersPack3DCJobId == fittersPackRequestModel.EducationTool3dcJobId)))
                .FirstOrDefault();

            if (version.IsNull())
            {
                _logger.LogDebug($"{version} not found, FittersPack3DCJobId = {fittersPackRequestModel.EducationTool3dcJobId}");
                return NotFound();
            }

            version.FittersPackPath = fittersPackRequestModel.EducationTool3dcFittersPackLocation;
            version.FittersPackStatusId = (int)Enum.Parse<FittersPackStatusEnum>(fittersPackRequestModel.EducationTool3dcFitterPackStatus);

            _versionRepository.Update(version);

            return new NoContentResult();
        }
    }
}

