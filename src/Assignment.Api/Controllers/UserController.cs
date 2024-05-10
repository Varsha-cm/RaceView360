using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Assignment.Api.Models;
using Assignment.Infrastructure.Repository;
using Assignment.Core.ThirdPartyModels;
using Assignment.Infrastructure.AuditLog;
using Assignment.Service.Model;
using Assignment.Service.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Emit;
using System.Security;
using System.Security.Claims;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using IdentityServer4.Validation;
using Assignment.Api.Interfaces;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("")]
    [ApiController]

    public class UserController : BaseController
    {
        private readonly UserService _userService;
        private readonly OrganizationService _organizationService;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        private readonly RolesPermissionService _roleService;
        private readonly ApplicationsService _applicationsService;
        private readonly IDBApplicationRepository _applicationsRepository;
        private readonly Serilog.Core.Logger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="applicationsService"></param>
        /// <param name="roleService"></param>
        /// <param name="authService"></param>
        /// <param name="configuration"></param>
        /// <param name="organizationService"></param>
        /// <param name="logger"></param>
        public UserController(UserService userService, ApplicationsService applicationsService, RolesPermissionService roleService, AuthService authService, IConfiguration configuration, OrganizationService organizationService, Serilog.Core.Logger logger) : base(logger)
        {
            _userService = userService;
            _organizationService = organizationService;
            _configuration = configuration;
            _authService = authService;
            _roleService = roleService;
            _applicationsService = applicationsService;
            this.logger = logger;

        }

        /// <summary>
        /// Creates new user under organization when orgcode is passed and creates new user under application when appcode is passed.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal Server Error</response>   
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("manage-app-users")]
        [HttpPost("/users/")]
        public async Task<UserRS> CreateUserAsync([FromBody] UserRQ userRequest)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userAppCode = await _applicationsService.GetAppCodeByEmailAsync(userEmail);
            if (userOrgCode != null)
            {
                foreach (var orgCode in userOrgCode)
                {
                    var appcodes = await _applicationsService.GetAppCodeByOrgCodeAsync(orgCode);
                    foreach (var appcode in appcodes)
                    {
                        userAppCode.Add(appcode);
                    }
                }
            }
            if (userRole.Contains(1) || userOrgCode.Contains(userRequest.orgcode) || userAppCode.Contains(userRequest.appcode))
            {
                var user = await _userService.CreateUserAsync(userRequest, userRole, userEmail);
                return user;
            }
            else
            {
                logger.Error("Code does not match or not authorized to create users.");
                throw new ArgumentException("Code does not match or not authorized to create users.");
            }
        }

        /// <summary>
        /// retrieves a users by orgcode
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>     
        [CustomAuthorize("view-users")]
        [HttpGet("/users")]
        public async Task<IActionResult> GetOrganizationUsersAsync([FromQuery] string orgcode, [FromQuery] string appcode)
        {
            try
            {
                string authorizationHeader = Request.Headers["Authorization"].ToString();
                string token = authorizationHeader.Replace("Bearer ", "");
                token = await _authService.DecryptJwt(token);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
                string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
                var userRole = await _roleService.GetRolesByEmail(userEmail);
                var userOrgcodes = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
                var userAppCodes = await _applicationsService.GetAppCodeByEmailAsync(userEmail);


                if (!string.IsNullOrEmpty(orgcode) && !string.IsNullOrEmpty(appcode))
                {
                    var appOrgCode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);
                    IEnumerable<OrgUserRS> organizationUsers = null;
                    IEnumerable<AppUserDetailsRS> applicationUsers = null;
                    if (userRole.Contains(1) || userOrgcodes.Contains(orgcode))
                    {
                        organizationUsers = await _userService.GetOrganizationUserAsync(orgcode);
                    }
                    if (userRole.Contains(1) || userOrgcodes.Contains(appOrgCode) || userAppCodes.Contains(appcode))
                    {
                        applicationUsers = await _userService.GetApplicationUsersByAppCodeAsync(appcode);
                    }
                    var result = new
                    {
                        OrgUser = organizationUsers,
                        AppUsers = applicationUsers
                    };

                    return Ok(result);
                }
                else if (!string.IsNullOrEmpty(orgcode))
                {
                    if (userRole.Contains(1) || userOrgcodes.Contains(orgcode))
                    {
                        var organizationUsers = await _userService.GetOrganizationUserAsync(orgcode);
                        List<AppUserDetailsRS> AppUsers = new List<AppUserDetailsRS>();
                        var appcodes = await _applicationsService.GetApplicationListRepositoryAsync(orgcode);
                        if (appcodes != null)
                        {
                            foreach (var app in appcodes)
                            {
                                var details = await _userService.GetApplicationUsersByAppCodeAsync(app.AppCode);
                                if (details != null)
                                {
                                    AppUsers.AddRange(details);
                                }
                            }
                        }

                        if (organizationUsers == null && AppUsers == null)
                        {
                            logger.Error("No users found for the provided orgcode.");
                            throw new KeyNotFoundException("No users found for the provided orgcode.");
                        }

                        var result = new
                        {
                            orgUsers = organizationUsers,
                            AppUsers = AppUsers,
                        };
                        return Ok(result);
                    }
                }
                else if (!string.IsNullOrEmpty(appcode))
                {
                    var appOrgCode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);
                    if (userRole.Contains(1) || userOrgcodes.Contains(appOrgCode) || userAppCodes.Contains(appcode))
                    {
                        var applicationUsers = await _userService.GetApplicationUsersByAppCodeAsync(appcode);
                        if (applicationUsers == null || !applicationUsers.Any())
                        {
                            logger.Error("No users found for the provided appcode.");
                            throw new KeyNotFoundException("No users found for the provided appcode.");
                        }
                        return Ok(applicationUsers);
                    }
                }
                else
                {
                    if (userRole.Contains(1))
                    {
                        var allOrgUsers = await _userService.GetOrganizationUsersAsync();
                        var allAppUsers = await _userService.GetApplicationUsersAsync();
                        var combined = new
                        {
                            OrgUser = allOrgUsers,
                            AppUsers = allAppUsers
                        };
                        return Ok(combined);
                    }
                    else if (userRole.Contains(2))
                    {
                        var OrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
                        List<IEnumerable<OrgUserRS>> OrgUsers = new List<IEnumerable<OrgUserRS>>();

                        foreach (var org in OrgCode)
                        {
                            var items = await _userService.GetOrganizationUsersAsync(org);
                            OrgUsers.Add(items);
                        }

                        var AppCode = await _applicationsService.GetAppCodeByEmailAsync(userEmail);
                        List<AppUserDetailsRS> AppUsers = new List<AppUserDetailsRS>();

                        foreach (var app in AppCode)
                        {
                            var items = await _userService.GetApplicationUsersByAppCodeAsync(app);
                            AppUsers.AddRange(items.Distinct());
                        }

                        var result = new
                        {
                            OrgUsers = OrgUsers.Distinct().ToList(),
                            AppUsers = AppUsers
                        };

                        return Ok(result);

                    }
                    else
                    {
                        var AppCode = await _applicationsService.GetAppCodeByEmailAsync(userEmail);
                        List<AppUserDetailsRS> AppUsers = new List<AppUserDetailsRS>();

                        foreach (var app in AppCode)
                        {
                            var items = await _userService.GetApplicationUsersByAppCodeAsync(app);
                            AppUsers.AddRange(items.Distinct());
                        }
                        return Ok(AppUsers);
                    }
                }

                throw new UnauthorizedAccessException("You don't have permission");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                logger.Error("Appcode not found");
                throw new ArgumentException("AppCode not found");
            }
        }

        /// <summary>
        /// Creates new user under application
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid AppCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>        
        //[CustomAuthorize("manage-app-users")]
        //[HttpPost("/users/application/{appcode}")]
        //public async Task<AppUserRQ> CreateAppUser([FromRoute] string appcode, [FromBody] AppUserRQ userRequest)
        //{
        //    string authorizationHeader = Request.Headers["Authorization"].ToString();
        //    string token = authorizationHeader.Replace("Bearer ", "");
        //    token = await _authService.DecryptJwt(token);
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
        //    string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

        //    var userRole = await _roleService.GetUserRoleAsync(userEmail);
        //    var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
        //    var appId = await _applicationsService.GetAppIdByAppCodeAsync(appcode);
        //    var apporgcode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);
        //    if (userRole == 1 || userOrgCode.Contains(apporgcode))
        //    {
        //        var user = await _userService.CreateAppUserAsync(userRequest, appcode, userEmail);

        //        var organizationInfo = await _organizationService.GetOrganizationByIdAsync(apporgcode);


        //        if (organizationInfo != null)
        //        {
        //            await _userService.UpdateApplicationUserAsync(user.UserId, organizationInfo.OrganizationId, organizationInfo.OrgCode, appcode, appId);
        //            await _userService.AddUserRoleAsync(user.UserId, 3, organizationInfo.OrganizationId, appId);
        //            return userRequest;
        //        }

        //        logger.Error("organizationinfo not found");
        //        throw new KeyNotFoundException("organizationinfo not found");
        //    }
        //    else
        //    {
        //        logger.Error("OrgCode does not match or not a super admin");
        //        throw new ArgumentException("OrgCode does not match or not a super admin");
        //    }
        //}

        ///<summary>
        /// Deactivates a user under organization by orgcode and userid.
        /// </summary>
        /// <param name="orgcode"></param>
        /// <param name="userid"></param>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid UserId or OrgCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("manage-org-users")]
        [HttpDelete("users/{userid}/organizations/{orgcode}")]
        public async Task<IActionResult> DeactivateUser([FromRoute] string orgcode, [FromRoute] int userid)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            await _userService.DeactivateUserAsync(orgcode, userid, userEmail);
            return Ok(new { Message = "User Deactivated Successfully" });
        }

        ///<summary>
        /// Deactivates a user under application by appcode and userid.
        /// </summary>
        /// <param name="appcode"></param>
        /// <param name="userid"></param>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid UserId or AppCode.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("manage-app-users")]
        [HttpDelete("users/{userid}/applications/{appcode}")]
        public async Task<IActionResult> DeactivateAppUser([FromRoute] string appcode, [FromRoute] int userid)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userRole = await _roleService.GetRolesByEmail(userEmail);
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userappcodes = await _applicationsService.GetAppCodeByEmailAsync(userEmail);
            var appId = await _applicationsService.GetAppIdByAppCodeAsync(appcode);
            var apporgcode = await _applicationsService.GetOrgCodeByAppCodeAsync(appcode);

            if (userRole.Contains(1) || userOrgCode.Contains(apporgcode) || userappcodes.Contains(appcode))
            {
                await _userService.DeactivateAppUserAsync(appcode, userid, userEmail);
                return Ok(new { Message = "User Deactivated Successfully" });

            }
            throw new UnauthorizedAccessException("YOU DO NOT HAVE PERMISSION");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [CustomAuthorize("manage-app-users")]
        [HttpPost("user/set-user-role")]
        public async Task<IActionResult> SetUserRole([FromBody] SetUserRoleRQ request)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string loggedInUserEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            try
            {
                await _userService.SetUserRole(request, loggedInUserEmail);
                return Ok(new { Message = "Role assigned successfully." });
            }
            catch (SecurityException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception)
            {
                logger.Error("An error occurred while assigning the role.");
                return StatusCode(500, new { Message = "An error occurred while assigning the role." });
            }
        }
    }
}




