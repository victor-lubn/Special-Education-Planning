using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Report Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportRepository reportRepository;

        private readonly ILogger<ReportController> logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="reportRepository"></param>
        public ReportController(IReportRepository reportRepository, ILogger<ReportController> logger)
        {
            this.reportRepository = reportRepository;

            this.logger = logger;
        }

        /// <summary>
        ///     Retrieve a report for the specified from and to dates
        /// </summary>
        /// <param name="fromDate">Area id</param>
        /// <param name="toDate">Area id</param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Report_Request)]
        public async Task<IActionResult> GetReport(DateTime fromDate, DateTime toDate)
        {
            logger.LogDebug("ReportController called GetReport");

            if (toDate.Subtract(fromDate).TotalDays > 90)
            {
                logger.LogDebug("ReportController end call GetReport -> return Bad request");

                return BadRequest("The maximum period among dates is 90 days");
            }
            var response = await reportRepository.GetReport(fromDate, toDate);

            logger.LogDebug("ReportController end call GetReport -> return Report");

            return response.GetHttpResponse();
        }
    }
}
