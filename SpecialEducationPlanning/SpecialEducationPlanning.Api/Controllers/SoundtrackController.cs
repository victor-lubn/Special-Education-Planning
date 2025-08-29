using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Configuration.OAuth;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Soundtrack Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SoundtrackController : Controller
    {
        private readonly ISoundtrackRepository soundtrackRepository;
        private readonly ILogger<SoundtrackController> logger;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="soundtrackRepository">Repository</param>
        /// <param name="logger">Controller logger</param>
        public SoundtrackController(ISoundtrackRepository soundtrackRepository
            , ILogger<SoundtrackController> logger, IObjectMapper mapper)
        {
            this.soundtrackRepository = soundtrackRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthorizeTdpFilter(PermissionType.Plan_Publish)]
        public async Task<IActionResult> GetAsync(int id)
        {
            logger.LogDebug("SoundtrackController called GetAsync");

            var repositoryResponse = new RepositoryResponse<SoundtrackModel>();
            var soundtrackResponseEntity =  await soundtrackRepository.FindOneAsync<Soundtrack>(id);
            var soundtrackModel = mapper.Map<Soundtrack, SoundtrackModel>(soundtrackResponseEntity);
            repositoryResponse.Content = soundtrackModel;
            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No Soundtrack found");

                logger.LogDebug("SoundtrackController end call GetAsync -> return Not found");

                return NotFound();
            }

            logger.LogDebug("SoundtrackController end call GetAsync -> return Soundtrack");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Retrieves a list of all available soundtracks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeTdpFilter(PermissionType.Plan_Publish)]
        public async Task<IActionResult> GetAllAsync()
        {
            logger.LogDebug("SoundtrackController called GetAllAsync");

            var repositoryResponse = new RepositoryResponse<IEnumerable<SoundtrackModel>>();
            var content = await soundtrackRepository.GetAllAsync<Soundtrack>();
            var model = mapper.Map<Soundtrack, SoundtrackModel>(content);
            repositoryResponse.Content = model;

            logger.LogDebug("SoundtrackController end call GetAllAsync -> return All soundtrack");

            return repositoryResponse.GetHttpResponse();
        }
    }
}