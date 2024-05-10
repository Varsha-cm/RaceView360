using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment.Api.Models;
using Assignment.Infrastructure.Repository;
using Assignment.Service.Model;
using Assignment.Service.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Emit;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>

    [ApiController]
    [Route("products")]
    public class ProductsController : BaseController
    {
        private readonly ProductsService _productsService;
        private readonly AuthService _authService;
        private readonly RolesPermissionService _rolePermissionService;
        private readonly ApplicationsService _applicationService;
        private readonly OrganizationService _organizationService;
        private readonly Serilog.Core.Logger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productsService"></param>
        /// <param name="organizationService"></param>
        /// <param name="applicationService"></param>
        /// <param name="rolePermissionService"></param>
        /// <param name="authService"></param>
        /// <param name="logger"></param>
        public ProductsController(ProductsService productsService,OrganizationService organizationService ,ApplicationsService applicationService, RolesPermissionService rolePermissionService, AuthService authService, Serilog.Core.Logger logger) : base(logger)
        {
            _productsService = productsService;
            _authService = authService;
            _applicationService = applicationService;
            _rolePermissionService = rolePermissionService;
            _organizationService = organizationService;
            this.logger = logger;
        }
        /// <summary>
        /// This API enables and disables notifications for the products.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal Server Error</response>   
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        [CustomAuthorize("enable-disable-product")]
        [HttpPut("enable-disable")]
        public async Task<IActionResult> ToggleProduct([FromBody] ProductsRQ requestModel)
        {
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Replace("Bearer ", "");
            token = await _authService.DecryptJwt(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenClaim = tokenHandler.ReadToken(token) as JwtSecurityToken;
            string userEmail = tokenClaim.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var orgcodes = await _organizationService.GetOrgCodeByEmailAsync(userEmail);
            var appOrgcode = await _applicationService.GetApplicationDetailsAsync(requestModel.AppCode);
            if (await _rolePermissionService.GetUserRoleAsync(userEmail) == 1 || orgcodes.Contains(appOrgcode.OrgCode))
            {
                if (appOrgcode != null)
                {
                    var success = await _productsService.ToggleProductAsync(requestModel, userEmail);
                    if (success)
                    {
                        logger.Information("success");
                        return Ok(new { Message = "successful." });
                    }

                    return NotFound(new { Message = "Product not found." });
                }
                else
                {
                    logger.Error("Not found");
                    throw new ArgumentException("Something went wrong");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission.");
            }

        }
    }

}

