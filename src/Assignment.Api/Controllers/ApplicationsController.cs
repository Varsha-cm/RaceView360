using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using Assignment.Api.Middleware;
using Assignment.Api.Models;
using Assignment.Core.ThirdPartyModels;
using Assignment.Infrastructure.AuditLog;
using Assignment.Infrastructure.Notification;
using Assignment.Service.Model;
using Assignment.Service.Services;
using Swashbuckle.Swagger.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("/applications")]
    [ApiController]
        public class ApplicationsController : BaseController
        {
            private readonly ApplicationsService _applicationsService;
            private readonly AuthService _authService;
            private readonly IConfiguration _configuration;
            private readonly RolesPermissionService _rolesPermissionService;
            private readonly OrganizationService _organizationService;
            private readonly LoggingService _loggingService;
            private readonly Serilog.Core.Logger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationsService"></param>
        /// <param name="authService"></param>
        /// <param name="configuration"></param>
        /// <param name="rolesPermissionService"></param>
        /// <param name="organizationservice"></param>
        /// <param name="logger"></param>
        public ApplicationsController(ApplicationsService applicationsService, AuthService authService, IConfiguration configuration, RolesPermissionService rolesPermissionService, OrganizationService organizationservice, Serilog.Core.Logger logger) : base(logger)
        {
                _applicationsService = applicationsService;
                _authService = authService;
                _configuration = configuration;
                _rolesPermissionService = rolesPermissionService;
                _organizationService = organizationservice;
                this.logger = logger;

        }


        /// <summary>
        /// Creates new Application.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal Server Error</response>   
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        //[Authorize(Roles = "AppAdmin")]
        [CustomAuthorize("app-management")]
        [HttpPost()]
        public async Task<AppDetailsRS> AddApplication([FromBody] ApplicationsRQ applicationRequest)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var orgcodes = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            if (await _rolesPermissionService.GetUserRoleAsync(userEmail) == 1 || orgcodes.Contains(applicationRequest.OrgCode))
            {
                var organizationInfo = await _organizationService.GetOrganizationByIdAsync(applicationRequest.OrgCode);
                if (organizationInfo != null)
                {
                    return await _applicationsService.AddApplicationAsync(applicationRequest, userEmail);
                }
                else
                {
                    logger.Error("Organization info not found for OrgCode: {0}", applicationRequest.OrgCode);
                    throw new ArgumentException("Something went wrong");
                }
            }
            else
            {
                logger.Error("OrgCode does not match or not a org admin");
                throw new ArgumentException("You do not have permission.");
            }

        }

        /// <summary>
        /// This end point is used for editing the application details excluding client id and client secret.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid AppCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("app-permissions-all")]
        [HttpPut("{appcode}")]
        public async Task<ApplicationsRQ> UpdateApplication([FromRoute] string appcode, [FromBody] UpdateApplicationRQ applicationRequest)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            var userRole = await _rolesPermissionService.GetUserRoleAsync(userEmail);
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userAppCode = await _applicationsService.GetAppCodeByEmailAsync(userEmail);
            var appOrgcode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);

            if (userRole == 3)
            {
                if (!userAppCode.Contains(appcode))
                {
                    logger.Error("Appcode does not match.");
                    throw new UnauthorizedAccessException("You do not have permission.");
                }
                userOrgCode = await _applicationsService.GetOrgCodeByEmailAsync(userEmail);
            }
            if (userRole == 1 || userOrgCode.Contains(appOrgcode) || userAppCode.Contains(appcode))
            {
                var organizationInfo = await _organizationService.GetOrganizationByIdAsync(appOrgcode);
                int existingAppId = await _applicationsService.GetAppIdByAppCodeAsync(appcode);

                if (existingAppId != 0 && organizationInfo != null)
                {
                    return await _applicationsService.UpdateApplicationAsync(applicationRequest, appcode, appOrgcode, userEmail);
                }
                else
                {
                    logger.Error("application info not found");
                    throw new KeyNotFoundException("application info not found");
                }
            }

            else
            {
                logger.Error("OrgCode does not match or not a org admin");
                throw new ArgumentException("OrgCode does not match or not a org admin");
            }


        }

        /// <summary>
        /// This end point is used for Fetching the application details and uses query params.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid OrgCode/AppCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("app-permissions-all")]
        [HttpGet("")]
        public async Task<IActionResult> GetApplicationList([FromQuery] string orgcode, [FromQuery] string appcode)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            if (!string.IsNullOrEmpty(orgcode))
            {
                orgcode = orgcode.ToUpper();
                var userRole = await _rolesPermissionService.GetRolesByEmail(userEmail);
                var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
                if (userRole.Contains(1) || userOrgCode.Contains(orgcode))
                {
                    var rs = await _applicationsService.GetApplicationListAsync(orgcode);
                    return Ok(rs);
                }
                else
                {
                    logger.Error("Does not belong to same organization");
                    throw new UnauthorizedAccessException("You donot have permission.");
                }
            }
            else if (!string.IsNullOrEmpty(appcode))
            {
                appcode = appcode.ToUpper();
                var userRole = await _rolesPermissionService.GetRolesByEmail(userEmail);
                var userOrgCode = await _applicationsService.GetOrgCodeByEmailAsync(userEmail);
                if (userRole.Contains(2))
                {
                    var orgCodes = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
                    foreach (var item in orgCodes)
                    {
                        userOrgCode.Add(item);
                    }
                }
                var appOrgcode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);
                if (userRole.Contains(1) || userOrgCode.Contains(appOrgcode))
                {
                    var rs = await _applicationsService.GetApplicationDetailsAsync(appcode);
                    return Ok(rs);
                }
                else
                {
                    logger.Error("Does not belong to same organization");
                    throw new UnauthorizedAccessException("You donot have permission.");
                }
            }
            else
            {
                logger.Error("Parameters are not specified");
                throw new UnauthorizedAccessException("Parameters are not specified");
            }
        }
        /// <summary>
        /// Deactivates an application by appcode.
        /// </summary>
        /// <param name="appcode"></param>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid AppCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("app-management")]
        [HttpDelete("{appcode}")]
        public async Task<IActionResult> DeactivateApplication([FromRoute] string appcode)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            await _applicationsService.DeactivateApplicationAsync(appcode, userEmail);
            LogInformation("Application deactivated successfully");
            return Ok(new { Message = "Application deactivated successfully" });
        }
    }
}
