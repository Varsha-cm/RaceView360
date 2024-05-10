using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment.Api.Models;
using Assignment.Service.Model;
using Assignment.Infrastructure.Notification;
using Assignment.Service.Services;
using Assignment.Api.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using System.Reflection.Emit;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("organizations")]
    [ApiController]
    public class OrganizationController : BaseController
    {
        private readonly OrganizationService _organizationService;
        private readonly AuthService _authService;
        private readonly RolesPermissionService _roleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="roleService"></param>
        /// <param name="authService"></param>
        /// <param name="logger"></param>
        public OrganizationController(OrganizationService organizationService, RolesPermissionService roleService, AuthService authService, Serilog.Core.Logger logger) : base(logger)
        {
            //_configuration = configuration;
            _organizationService = organizationService;
            _authService = authService;
            _roleService = roleService;
        }

        /// <summary>
        ///  Retrieves the List of Organizations.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        [CustomAuthorize("org-permissions-all")]
        [HttpGet("list")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<OrgDetailsRS>> ListOrganizations()
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            if (userRole.Contains(1))
            {
                return await _organizationService.GetOrganizationsAsync();
            }
            else if (userRole.Contains(2))
            {
                var op = new List<OrgDetailsRS>();
                foreach (var item in userOrgCode)
                {
                    op.Add(await _organizationService.GetOrganizationByIdAsync(item));
                }
                return op;
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission");
            }
        }

        /// <summary>
        ///  Retrieves Organization details by OrgCode.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid OrgCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin, OrgAdmin")]
        [CustomAuthorize("org-permissions-all")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden)]

        [HttpGet("{orgcode}")]
        public async Task<OrgDetailsRS> GetOrganizationDetails([FromRoute] string orgcode)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            if ((userRole.Contains(1) || userOrgCode.Contains(orgcode)))
            {
                return await _organizationService.GetOrganizationByIdAsync(orgcode);
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission");
            }

        }

        /// <summary>
        /// Updates the existing organization record by orgcode.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid OrgCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin, OrgAdmin")]
        [CustomAuthorize("org-permissions-all")]
        [HttpPut("{orgcode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden)]
        public async Task<OrgDetailsRS> EditOrganizationDetails([FromRoute] string orgcode, [FromBody] UpdateOrganizationRQ model)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            if ((userRole.Contains(1) || userOrgCode.Contains(orgcode)))
            {
                    return await _organizationService.EditOrganizationDetailsAsync(orgcode, model,userEmail);
            }
            else
            {
                throw new UnauthorizedAccessException("You donot have permission.");
            }

        }

        /// <summary>
		/// Creates new Organization.
		/// </summary>
		/// <param name="organizationRequest">Input data</param>
		/// <returns>
		/// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
		/// </returns>
        //[Authorize(Roles = "SuperAdmin")]
        [CustomAuthorize("org-management")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden)]
        public async Task<OrgDetailsRS> AddOrganization([FromBody] OrganizationRQ organizationRequest)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            if (userRole.Contains(1))
            {
                var rs = await _organizationService.AddOrganizationAsync(organizationRequest, userEmail);
                return rs;
            }
            else
            {
                throw new UnauthorizedAccessException("YOU DONOT HAVE PERMISSION");
            }
        }

        /// <summary>
        /// Deactivates Organization by Id.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid OrgCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin")]
        [CustomAuthorize("org-management")]
        [HttpDelete("{orgcode}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> DeactivateOrganization([FromRoute] string orgcode)
        {

            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            if (userRole.Contains(1))
            {
                await _organizationService.DeactivateOrganizationAsync(orgcode, userEmail);
                return Ok(new { Message = "Organization deactivated successfully." });
            }
            else
            {
                throw new UnauthorizedAccessException("YOU DONOT HAVE PERMISSION");
            }
        }
    }
}
