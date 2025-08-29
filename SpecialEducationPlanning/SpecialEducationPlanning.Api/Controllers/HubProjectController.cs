using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.ProjectService;
using SpecialEducationPlanning
.Business.DtoModel;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     HubProject Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HubProjectController : Controller
    {
        private readonly ILogger<HubProjectController> logger;
        private readonly IProjectService creatioService;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Action Repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="automaticRemoveActionOptions"></param>
        public HubProjectController(
            ILogger<HubProjectController> logger,
            IProjectService creatioService
        )
        {
            this.logger = logger;
            this.creatioService = creatioService;
        }

        /// <summary>
        ///     Create/Update a project
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthorizeTdpFilter(PermissionType.Plan_Create)]
        public async Task<IActionResult> Post([FromHeader(Name = "Api-Version")] string apiVersion, 
                                              [FromHeader(Name = "App-Name")] [Required] string appName,
                                              [FromBody] CreatioProjectDto creatio)
        {
            logger.LogDebug("HubProjectController called Post -> Create/Update a project");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("HubProjectController end call Post -> return Bad request");

                return BadRequest(ModelState.GetErrorMessages());
            }

            var response = await creatioService.CreateUpdateProjectForCreatio(creatio);
            logger.LogDebug("ProjectController end call Post -> return Created project");

            return response.GetHttpResponse();
        }
    }
}
