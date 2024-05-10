using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment.Api.Models;
using Assignment.Service.Services;
using Assignment.Service.Model;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("roles")]
    [ApiController]
    public class RolesPermissionsController : BaseController
    {
        private readonly RolesPermissionService _rolePermissionService;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rolePermissionService"></param>
        /// <param name="configuration"></param>
        /// <param name="authService"></param>
        /// <param name="logger"></param>
        public RolesPermissionsController(RolesPermissionService rolePermissionService,IConfiguration configuration,AuthService authService, Serilog.Core.Logger logger) :base(logger)
        {
            _rolePermissionService = rolePermissionService;
            _configuration = configuration;
            _authService = authService;
        }

        /// <summary>
        ///  Retrieves the List of Roles.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin, OrgAdmin, AppAdmin")]
        [CustomAuthorize("view-roles-permissions")]
        [HttpGet("list")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IEnumerable<ListRolesRS>> ListRoles()
        {
            return _rolePermissionService.GetAllRoles();
        }

        /// <summary>
        ///  Retrieves the List of Permission of a Role by RoleId.
        /// </summary>
        ///  /// <param name="roleId"></param>
        /// <returns> 
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid RoleId.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin, OrgAdmin, AppAdmin")]
        [CustomAuthorize("view-roles-permissions")]
        [HttpGet("permissions/list/{roleId}")]       
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<ListPermissionsRS>> ListPermissions([FromRoute] int roleId)
        {    
            return _rolePermissionService.GetPermissionsByRole(roleId);
        }
    }
}
