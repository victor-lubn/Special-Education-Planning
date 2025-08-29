using Koa.Hosting.AspNetCore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Api.Service.CsvFile;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     CsvFile Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CsvFileController : ControllerBase
    {
        private readonly ILogger<CsvFileController> logger;
        private readonly ICsvFileService service;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="csvFileService"></param>
        public CsvFileController(ILogger<CsvFileController> logger, ICsvFileService csvFileService)
        {
            this.logger = logger;
            this.service = csvFileService;
        }

        /// <summary>
        /// Upload .csv file method
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        [HttpPost("{entity}/Csv")]
        [AuthorizeTdpFilter(PermissionType.Structure_Management)]
        public async Task<IActionResult> UploadCsv(string entity, [FromBody] MultiUploadedFileModel fileUpload)
        {
            logger.LogDebug("CsvFileController called UploadCsv");

            var response = await service.DumpCsv(entity, fileUpload);

            logger.LogDebug("CsvFileController end call UploadCsv -> return Response");

            return response.GetHttpResponse();
        }
    }
}