using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Service.Model;
using Assignment.Service.Services;
using System.IdentityModel.Tokens.Jwt;
using static IdentityServer4.Models.IdentityResources;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("/googleworkspace")]
    [ApiController]
    public class GoogleWorkSpaceController : BaseController
    {
        private readonly ApplicationsService _applicationsService;
        private readonly AuthService _authService;
        private readonly RolesPermissionService _rolesPermissionService;
        private readonly OrganizationService _organizationService;
        private readonly UserService _userService;
        private readonly GoogleWorkSpaceService _GoogleWorkSpaceService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INotificationService _notificationService;
        private readonly Serilog.Core.Logger logger;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationsService"></param>
        /// <param name="notificationService"></param>
        /// <param name="userService"></param>
        /// <param name="organizationService"></param>
        /// <param name="authService"></param>
        /// <param name="rolesPermissionService"></param>
        /// <param name="googleWorkSpaceService"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
    
        public GoogleWorkSpaceController(ApplicationsService applicationsService, INotificationService notificationService,UserService userService,OrganizationService organizationService,AuthService authService,RolesPermissionService rolesPermissionService,GoogleWorkSpaceService googleWorkSpaceService, IHttpClientFactory httpClientFactory, Serilog.Core.Logger logger) : base(logger)
        {
            _applicationsService = applicationsService;
            _authService = authService;
            _GoogleWorkSpaceService = googleWorkSpaceService;
            _rolesPermissionService = rolesPermissionService;
            _organizationService = organizationService;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
            _notificationService = notificationService;
            this.logger = logger;
        }

        /// <summary>
		/// Used to add domain for respective application or organization. Accepts appcode or orgcode. Specifying both is invalid. for superadmin domain pass appcode and orgcode as null.
		/// </summary>
		/// <returns>
		/// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
		/// </returns>
        [HttpPost("add-domain")]
        public async Task<AddDomainRQ> AddDomain([FromBody]AddDomainRQ rq)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userappCode = await _applicationsService.GetOrgCodeByEmailAsync(userEmail);
            var tokenUserRole = await _rolesPermissionService.GetRolesByEmail(userEmail);
            if (tokenUserRole.Contains(1) || userOrgCode.Contains(rq.OrgCode) || userappCode.Contains(rq.AppCode))
            { 
                var rs = await _GoogleWorkSpaceService.AddDomain(rq);
                return rs;
            }
            else
            {
                logger.Error("You dont have permission");
                throw new UnauthorizedAccessException("you donot have permissions.");
            }
        }

        /// <summary>
        /// Once user accepts the invite they can generate access token and refreshtoken from this endpoint using google sso.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Access token is required.");
            }
            using var client = _httpClientFactory.CreateClient();

            string act = token;
            string url = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={act}";
            var email = await client.GetAsync(url);

            if (!string.IsNullOrEmpty(email.ToString()))
            {
                var userInfo = await email.Content.ReadAsStringAsync();
                var deserializeData = JsonConvert.DeserializeObject<UserInfoRS>(userInfo);

                string userEmail = deserializeData.email;
                var userData = await _userService.GetUserByEmailAsync(userEmail);
                
                if (userData != null)
                {
                    var googleSigninData = await _GoogleWorkSpaceService.GetGoogleSignInDetails(userData.UserId);
                    if (googleSigninData != null && googleSigninData.HasAcceptedInvite)
                    {
                        var AccessToken = _authService.GenerateJwtToken(userEmail);
                        var refreshToken = _authService.GenerateRefreshToken();
                        await _authService.SaveRefreshToken(userEmail, refreshToken);

                        return Ok(new { AccessToken = AccessToken.Result, refreshToken = refreshToken });
                    }
                }
            }

            return BadRequest("Something went wrong");
        }
        /// <summary>
        /// after recieveing invite user can sign in to their google account and execute this api. So he will be able to generate access token from login api.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        [HttpPost("accept-invite")]
        public async Task<IActionResult> acceptInvite()
        {
            try
            {
                string authorizationHeader = Request.Headers["Authorization"].ToString();
                string token = authorizationHeader.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Access token is required.");
                }
                using var client = _httpClientFactory.CreateClient();
                string act = token;
                string url = $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={act}";
                var email = await client.GetAsync(url);
                logger.Information("email information from token is : "+email.Content.ReadAsStringAsync());
                if (!string.IsNullOrEmpty(email.ToString()))
                {
                    var userInfo = await email.Content.ReadAsStringAsync();
                    var deserializeData = JsonConvert.DeserializeObject<UserInfoRS>(userInfo);

                    string userEmail = deserializeData.email;
                    var userData = await _userService.GetUserByEmailAsync(userEmail);

                   
                    if (userData != null)
                    {
                        var isinvited = await _GoogleWorkSpaceService.IsInvited(userData.UserId);
                        logger.Information("email isInvited value : " + isinvited + "user data : " + userData.Email);

                        if(isinvited)
                        {
                            var rs = await _GoogleWorkSpaceService.AcceptInvite(userData.UserId);
                            logger.Information("Accept invite rs value :" + rs);
                            return Ok("accepted invite successfully");
                        }                   
                    }
                    logger.Error("not able to accept the invite - useremail is"+userEmail+"useremail from the google token is "+userEmail);
                    return BadRequest("Something went wrong");
                }
                else
                {
                    logger.Error("not able to accept the invite - email details are "+email.ToString());
                    return BadRequest("Something went wrong");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return BadRequest("Something went wrong");
            }
        }
        /// <summary>
        /// This performs hard delete on GoogleSignIn table to remove the Signin option for the user.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        [CustomAuthorize("app-management")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> CancelInvite([FromRoute]string email)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");

            token = await _authService.DecryptJwt(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;

            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userappCode = await _applicationsService.GetOrgCodeByEmailAsync(userEmail);

            var userEmailOrgCode = await _organizationService.GetOrgCodeByEmailAsync(email);
            var userEmailAppCode = await _applicationsService.GetAppCodeByEmailAsync(email);

            var tokenUserRole = await _rolesPermissionService.GetRolesByEmail(userEmail);

            var orgcodeIntersect = userOrgCode.Intersect(userEmailOrgCode).ToList();
            var appcodeIntersect = userappCode.Intersect(userEmailAppCode).ToList();

            if ((tokenUserRole.Contains(1) || orgcodeIntersect != null || appcodeIntersect != null))
            {
                
                var rs = await _GoogleWorkSpaceService.DeleteUserGoogleWorkSpace(email);
                if (rs == 1)
                {
                    return Ok("user Deleted Successfully");
                }
                else
                {
                    throw new ArgumentException("user already deleted");
                }
            }
            return BadRequest("Something went wrong");
        }
        /// <summary>
        /// This endpoint uses cyrax to send the invite for the google sign in for the particular user.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Something went wrong.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        [CustomAuthorize("app-management")]
        [HttpPost("invite-user/{email}")]
        public async Task<IActionResult> SendInvite(string email)
        {
            var userData = await _userService.GetUserByEmailAsync(email);
          
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");

            token = await _authService.DecryptJwt(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
          
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            
            var userOrgCode = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var userappCode = await _applicationsService.GetOrgCodeByEmailAsync(userEmail);
            
            var userEmailOrgCode = await _organizationService.GetOrgCodeByEmailAsync(email);
            var userEmailAppCode = await _applicationsService.GetAppCodeByEmailAsync(email);
            
            var tokenUserRole = await _rolesPermissionService.GetRolesByEmail(userEmail);
            
            var orgcodeIntersect = userOrgCode.Intersect(userEmailOrgCode).ToList();
            var appcodeIntersect = userappCode.Intersect(userEmailAppCode).ToList();
            
            if ((tokenUserRole.Contains(1) || orgcodeIntersect != null || appcodeIntersect != null))
            {
                var userdomainallowed = await _GoogleWorkSpaceService.IdDomainAllowed(email);
                if (userData != null && userdomainallowed)
                {
                    var isInvited = await _GoogleWorkSpaceService.IsInvited(userData.UserId);
                    if (isInvited)
                    {
                        logger.Error("User already invited");
                        throw new ArgumentException("user already invited");
                    }
                    var isAdded = await _GoogleWorkSpaceService.AddUserToGoogleSignIn(userData.UserId);
                    if (isAdded != 0)
                    {   
                        var response = await _notificationService.SendGoogleInviteAsync(email, "CYRAX_GOOGLE_INVITE");
                        return Ok(response);
                    }
                    else
                    {
                        logger.Error("Error while inviting the user");
                        throw new InvalidOperationException("Error while inviting the user");
                    }
                }
                else
                {
                    throw new InvalidOperationException("user doesnot exist or Domain not allowed");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("You donot have permission.");
            }
            }

    }
}
