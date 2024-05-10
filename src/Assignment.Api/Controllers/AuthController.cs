using Azure.Core;
using IdentityModel.Client;
using IdentityServer4;
using Jose;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Infrastructure.Repository;
using Assignment.Service.Model;
using Assignment.Service.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.Emit;
using System.Security;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static IdentityServer4.Models.IdentityResources;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [Route("auth")]
    [ApiController]
    public class AuthController : BaseController
    {

        private readonly AuthService _authService;
        private readonly RolesPermissionService _rolePermissionService;
        private readonly IConfiguration _configuration;
        private readonly OrganizationService _organizationService;
        private readonly ApplicationsService _applicationService;
        private readonly IDBApplicationRepository _applicationRepository;
        private readonly IDBProductsRepository _productRepository;
        private readonly IDBOrganization _organizationRepository;
        private readonly Serilog.Core.Logger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="organizationRepository"></param>
        /// <param name="productRepository"></param>
        /// <param name="applicationService"></param>
        /// <param name="applicationRepository"></param>
        /// <param name="rolePermissionService"></param>
        /// <param name="organizationService"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public AuthController(AuthService authService, IDBOrganization organizationRepository, IDBProductsRepository productRepository, ApplicationsService applicationService, IDBApplicationRepository applicationRepository, RolesPermissionService rolePermissionService, OrganizationService organizationService, Serilog.Core.Logger logger, IConfiguration configuration) : base(logger)
        {
            _authService = authService;
            _configuration = configuration;
            _rolePermissionService = rolePermissionService;
            _organizationService = organizationService;
            _applicationService = applicationService;
            _applicationRepository = applicationRepository;
            _organizationRepository = organizationRepository;
            _productRepository = productRepository;
            this.logger = logger;
        }

        /// <summary>
        /// User should provide the Email and password as login credentials.This endpoint gives Refresh token and access token as the response.
        /// </summary>
        /// <returns>
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid Credentials.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        //[Authorize(Roles = "SuperAdmin, OrgAdmin, AppAdmin")]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRQ authRequest)
        {
            var result = await _authService.AuthenticateAsync(authRequest);
            if (result <= 0)
            {
                logger.Error("Invalid email or password or no role exist for this user");
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = await _authService.GenerateJwtToken(authRequest.Email);
            var refreshToken = _authService.GenerateRefreshToken();
            await _authService.SaveRefreshToken(authRequest.Email, refreshToken);

            return Ok(new { Token = accessToken, RefreshToken = refreshToken });
        }

        /// <summary>
        ///  User have to provide the Email and refresh token to get the new access token and refresh token.
        /// </summary>
        /// <returns> 
        /// 200 OK - Success.
        /// <response code="400">Bad Request - Invalid Credentials.</response> 
        /// <response code="500"> Internal Server Error - An error occurred during data retrieval.</response>
        /// <response code="401"> Unauthorized - User don't have access.</response>
        /// <response code="403"> Forbidden - The User is Forbidden.</response>
        /// </returns>
        [AllowAnonymous]
        [HttpPost("refresh")]        
        public async Task<IActionResult> Refresh([FromBody] RefreshRQ refreshRequest)
        {
            int flag = Convert.ToInt32(await _authService.IsRefreshTokenValid(refreshRequest.RefreshToken, refreshRequest.Email));
            if (flag == 0)
                throw new UnauthorizedAccessException("Invalid refresh token");
            var refreshToken = _authService.GenerateRefreshToken();
            await _authService.SaveRefreshToken(refreshRequest.Email, refreshToken);
            var accessToken = await _authService.GenerateJwtToken(refreshRequest.Email);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost("decrypt-token")]
        public IActionResult DecryptToken([FromBody] string token)
        {
            try
            {
                string decryptedToken = _authService.DecryptJwt(token).Result;
                return Ok(decryptedToken);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AuthenticationException authEx)
            {
               
                return Unauthorized(authEx.Message);
            }
           
        }

        //privates
        //private async Task<string> GenerateJwtToken(string email)
        //{
           
        //    var encryptedJwt = await _authService.GenerateJwtToken(email);

        //    return encryptedJwt;

        //}


        //private static string GenerateRefreshToken()
        //{
        //    var 
        //    var randomNumber = new byte[32];
        //    using var rng = RandomNumberGenerator.Create();
        //    rng.GetBytes(randomNumber);
        //    return Convert.ToBase64String(randomNumber);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("generate-application-token")]
        public async Task<IActionResult> GenerateToken([FromBody] ClientTokenRQ model)
        {

            int authenticationResult = _authService.AuthenticateApplication(model.ClientId, model.ClientSecret);

            if (authenticationResult > 0)
            {
                var appcode = await _applicationRepository.GetAppCodeByClientIdAsync(model.ClientId);
                string token = await GenerateApplicationTokenInternal(appcode);
                return Ok(new { Token = token });
            }
            else
            {

                return Unauthorized(new { Message = "Authentication failed" });
            }
        }

        private async Task<string> GenerateApplicationTokenInternal(string appcode)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "JWTToken"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("appcode", appcode),
            new Claim("orgcode", await _applicationService.GetOrgCodeByAppCodeAsync(appcode)),
        };
            await _productRepository.GetProductsAsync();
            var servicestatus = await _productRepository.GetStatusOfProductAsync(appcode);
            var appPermissions = servicestatus
                .Select(product => $"{product.Products.ProductName}::{(product.IsEnabled ? "enabled" : "disabled")}").ToList();
            claims.Add(new Claim("services", JsonConvert.SerializeObject(appPermissions), JsonClaimValueTypes.JsonArray));

            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("ValidIssuer"),
                Environment.GetEnvironmentVariable("ValidIssuer"),
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );           
            var encryptedJwt = _authService.EncryptJwt(token);

            return encryptedJwt;

        }

    }
}
