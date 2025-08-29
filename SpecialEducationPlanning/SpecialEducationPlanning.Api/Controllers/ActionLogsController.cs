using Koa.Domain.Search.Page;
using Koa.Hosting.AspNetCore.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Model.AutomaticRemoveItems;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Action Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ActionLogsController : Controller
    {
        private readonly IActionLogsRepository repository;
        private readonly ILogger<ActionLogsController> logger;
        private readonly IOptions<AutomaticRemoveActionConfiguration> automaticRemoveActionOptions;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repository">Action Repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="automaticRemoveActionOptions"></param>
        public ActionLogsController(IActionLogsRepository repository,
            ILogger<ActionLogsController> logger,
            IOptions<AutomaticRemoveActionConfiguration> automaticRemoveActionOptions
        )
        {
            this.repository = repository;
            this.logger = logger;
            this.automaticRemoveActionOptions = automaticRemoveActionOptions;
        }

        #region Remove Old Log Item
        /// <summary>
        ///     Automatically remove log items greater than 90 days
        /// </summary>
        /// <returns></returns>
        [HttpDelete("AutomaticRemoveOldItems")]
        [AuthorizeTdpFilter(PermissionType.Data_Management)]
        public async Task<IActionResult> AutomaticRemoveOldItems()
        {
            logger.LogDebug("ActionLogsController called AutomaticRemoveOldItems");

            var delete = double.Parse(automaticRemoveActionOptions.Value.Delete, CultureInfo.InvariantCulture);
            await repository.AutomaticRemoveOldItems(DateTime.UtcNow, delete);

            logger.LogDebug("ActionLogsController end call AutomaticRemoveOldItems -> returns Ok");

            return Ok();
        }
        #endregion

        /// <summary>
        ///     Retrieve action logs based on a filter
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetActionLogsFiltered")]
        public async Task<IActionResult> GetActionLogsFiltered([FromBody] PageDescriptor searchModel)
        {
            logger.LogDebug("ActionLogsController called GetActionsLogsFiltered");

            if (!ModelState.IsValid)
            {
                logger.LogDebug("ModelState is not valid {errorMessages}", ModelState.GetErrorMessages().Join(System.Environment.NewLine));

                logger.LogDebug("ActionLogsController end call GetActionLogsFiltered -> return Model Error");

                return BadRequest(ModelState.GetErrorMessages());
            }

            //TODO DEFAULT TAKE VALUE FROM SETTINGS
            if (!searchModel.Take.HasValue || searchModel.Take.Value > 500)
            {
                searchModel.Take = 500;
            }

            var models = await repository.GetActionLogsFilteredAsync(searchModel);

            if (models.ErrorList.Any())
            {
                logger.LogError("Error found: {erros}", models.ErrorList.Join("/"));

                logger.LogDebug("ActionLogsController end call GetActionsLogsFiltered -> return List Error");

                return models.GetHttpResponse();
            }

            logger.LogDebug("ActionLogsController end call GetActionsLogsFiltered -> return Paged action logs");

            return this.PagedJsonResult(models.Content, true);
        }

        /// <summary>
        ///     Retrieve actions logs based on start and end dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActionLogsCsv")]
        public async Task<IActionResult> GetActionLogsCsv(string startDate, string endDate)
        {
            logger.LogDebug("ActionLogsController call GetActionLogsCsv");

            var sb = await repository.GetActionLogsCsv(DateTime.Parse(startDate), DateTime.Parse(endDate));

            if (sb.Content.Length == 0)
            {
                logger.LogDebug("There are no action logs");

                logger.LogDebug("ActionLogsController end call GetActionLogsCsv -> return No Content");

                return NoContent();
            }

            logger.LogDebug("ActionLogsController end call GetActionLogsCsv -> return Content");

            return File(new UTF8Encoding().GetBytes(sb.Content.ToString()), "text/csv", "actionLogs.csv");
        }
    }
}