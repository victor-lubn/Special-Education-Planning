using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Api.Extensions;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Entity;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpecialEducationPlanning
.Api.Controllers
{
    /// <summary>
    ///     Permission Controller
    /// </summary>
    [Route("api/[controller]")]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository permissionRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ILogger<PermissionController> logger;
        private readonly IObjectMapper mapper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="permissionRepository">Repository</param>
        /// <param name="roleRepository">Role Repository</param>
        /// <param name="logger">Logger</param>
        public PermissionController(IPermissionRepository permissionRepository
            , IRoleRepository roleRepository
            , ILogger<PermissionController> logger,
            IObjectMapper mapper)
        {
            this.permissionRepository = permissionRepository;
            this.roleRepository = roleRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        ///     Get Permission by ID
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>If the ID is found returns the model, if not, it returns a NotFound</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogDebug("PermissionController called Get");

            var permission = await permissionRepository.FindOneAsync<Permission>(id);
            var permissionModel = mapper.Map<Permission, PermissionModel>(permission);
            RepositoryResponse<PermissionModel> repositoryResponse = new RepositoryResponse<PermissionModel>
            {
                Content = permissionModel
            };

            if (repositoryResponse.Content.IsNull())
            {
                logger.LogDebug("No permission found");

                logger.LogDebug("PermissionController end call Get -> return Not found");

                return NotFound();
            }

            logger.LogDebug("PermissionController end call Get -> return Permission");

            return repositoryResponse.GetHttpResponse();
        }

        /// <summary>
        ///     Get all Permissions
        /// </summary>
        /// <returns>Collection of Permissions</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogDebug("PermissionController called GetAll");
            var permissions = await permissionRepository.GetAllAsync<Permission>();
            var permissionModels = mapper.Map<IEnumerable<Permission>, IEnumerable<PermissionModel>>(permissions);
            RepositoryResponse<IEnumerable<PermissionModel>> repositoryResponse = new RepositoryResponse<IEnumerable<PermissionModel>>
            {
                Content = permissionModels
            };

            logger.LogDebug("PermissionController end call GetAll -> return All permissions");

            return repositoryResponse.GetHttpResponse();
        }
    }
}