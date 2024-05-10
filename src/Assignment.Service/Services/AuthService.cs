using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Amazon.Runtime.Internal.Util;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;

namespace Assignment.Service.Services
{
    public class AuthService
    {
        private readonly IDBAuthRepository _authRepository;
        private readonly IDBApplicationRepository _applicationRepository;
        private readonly IDBOrganization _organizationRepository;
        private readonly RolesPermissionService _rolePermissionService;
        private readonly OrganizationService _organizationService;
        private readonly ApplicationsService _applicationService;
        public AuthService(IDBAuthRepository authRepository,IDBOrganization organizationRepository,IDBApplicationRepository applicationRepository ,RolesPermissionService rolePermissionService, OrganizationService organizationService, ApplicationsService applicationService)
        {
            _authRepository = authRepository;
            _rolePermissionService = rolePermissionService;
            _organizationService = organizationService;
            _applicationService = applicationService;   
            _applicationRepository = applicationRepository;
            _organizationRepository = organizationRepository;
        }
        public async Task<int> AuthenticateAsync(AuthRQ authRequest)
        {

            var ret = await _authRepository.AuthenticateAsync(authRequest.Email, authRequest.Password);
            return ret;
        }

        public async Task<int> IsRefreshTokenValid(string refreshToken, string username)
        {
            return await _authRepository.IsRefreshTokenValid(refreshToken, username);
        }

        public async Task SaveRefreshToken(string username, string refreshToken)
        {
            await _authRepository.SaveRefreshToken(username, refreshToken);
        }

        public async Task<string> GetRefreshToken(string username)
        {
            return await _authRepository.GetRefreshToken(username);
        }

        public async Task RemoveRefreshToken(string username)
        {
            await _authRepository.RemoveRefreshToken(username);
        }
        public int AuthenticateApplication(string clientID, string clientSecret)
        {
            return _authRepository.AuthenticateApplication(clientID, clientSecret);
        }        

        public bool IsAuthorized(string token, string requiredAction, string requiredPermission, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            var claims = (validatedToken as JwtSecurityToken).Claims;
            var permissionsClaim = claims.FirstOrDefault(claim => claim.Type == "permissions");
            var organizationsClaim = claims.FirstOrDefault(claim => claim.Type == "organizations");
            var applicationsClaim = claims.FirstOrDefault(claim => claim.Type == "applications");

            if (permissionsClaim != null)
            {
                var hasRequiredPermission = (validatedToken as JwtSecurityToken)
                                             .Claims
                                            .Any(claim => claim.Type == "permissions" && claim.Value.Contains($"{requiredAction}::{requiredPermission}"));

                return hasRequiredPermission;
            }

            else if (organizationsClaim != null)
            {
                var orgDataArray = JArray.Parse(organizationsClaim.Value);

                foreach (var orgData in orgDataArray)
                {
                    if (orgData is JObject orgObject && orgObject.ContainsKey("permissions"))
                    {
                        var orgPermissions = orgObject["permissions"].ToObject<List<string>>();
                        var requiredClaim = $"{requiredAction}::{requiredPermission}";
                        if (orgPermissions.Contains(requiredClaim))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            else if (applicationsClaim != null)
            {
                var appDataArray = JArray.Parse(applicationsClaim.Value);
                if (appDataArray.Count > 0)
                {
                    foreach (var appData in appDataArray)
                    {
                        if (appData is JObject appObject && appObject.ContainsKey("permissions"))
                        {
                            var appPermissions = appObject["permissions"].ToObject<List<string>>();
                            var requiredClaim = $"{requiredAction}::{requiredPermission}";
                            if (appPermissions.Contains(requiredClaim))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedtoken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> DecryptJwt(string encryptedtoken)
        {
            if (string.IsNullOrEmpty(encryptedtoken))
            {
                throw new ArgumentException("Token is missing or empty.");
            }

            var privatekeyPath = Environment.GetEnvironmentVariable("PrivateKeyPath");
            string certificatePassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");

            try
            {
                X509Certificate2 certificate = new X509Certificate2(privatekeyPath, certificatePassword);

                if (!certificate.HasPrivateKey)
                {
                    throw new ArgumentException("The certificate does not contain a private key.");
                }                
                using (RSA rsaPrivateKey = certificate.GetRSAPrivateKey())
                {
                    string decryptedJwt = JWT.Decode(encryptedtoken, rsaPrivateKey, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);
                    decryptedJwt = decryptedJwt.Trim('"');
                    decryptedJwt = decryptedJwt.Trim('\\');                   

                    return decryptedJwt; 
                }
            }
            catch (CryptographicException cryptoEx)
            {
                throw new CryptographicException("Cryptographic exception: " + cryptoEx.Message+"\n Path :"+privatekeyPath);
            }
        }
        public async Task<string> GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("Subject")),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };
            var NullList = new List<Claim>();
            var roleids = await _rolePermissionService.GetRolesByEmail(email);
            if (roleids.Contains(1))
            {
                var userPermissions = _rolePermissionService.GetPermissionsByRole(1);
                foreach (var action in userPermissions)
                {
                    foreach (var permission in action.Permissions)
                        claims.Add(new Claim("permissions", action.ActionName + "::" + permission.PermissionName));

                }
            }
            claims.Add(new Claim("email", email));

            if (roleids.Contains(3))
            {
                var userPermissions = _rolePermissionService.GetPermissionsByRole(3);
                var appcodes = await _applicationService.GetAppCodeByEmailAsync(email);
                var appClaims = new List<ApplicationsClaim>();

                foreach (var appcode in appcodes)
                {
                    var appinfo = await _applicationRepository.GetApplicationDetailsAsync(appcode);
                    if (appinfo.IsActive == true)
                    {
                        var appPermissions = userPermissions
                        .SelectMany(action => action.Permissions, (action, permission) => $"{action.ActionName}::{permission.PermissionName}")
                        .ToList();
                        var orgInfo = await _organizationRepository.GetOrganizationByIdAsync(await _applicationService.GetOrgCodeByAppCodeAsync(appcode));
                        if (orgInfo != null)
                        {
                            var orgemail = orgInfo.OrganizationEmail;

                            var appClaim = new ApplicationsClaim
                            {
                                appcode = appcode,
                                orgcode = await _applicationService.GetOrgCodeByAppCodeAsync(appcode),
                                permissions = appPermissions,
                                applicationemail = await _applicationRepository.GetEmailByAppCodeAsync(appcode),
                                organizationemail = orgemail
                            };

                            appClaims.Add(appClaim);
                        }
                    }
                }
                claims.Add(new Claim("applications", "[" + JsonConvert.SerializeObject(appClaims) + "]", JsonClaimValueTypes.JsonArray));
            }

            if (roleids.Contains(2))
            {
                var userPermissions = _rolePermissionService.GetPermissionsByRole(2);
                var orgcodes = await _organizationService.GetOrgCodeByEmailAsync(email);

                var orgPermissions = userPermissions
                    .SelectMany(action => action.Permissions, (action, permission) => $"{action.ActionName}::{permission.PermissionName}")
                    .ToList();

                var orgClaims = new List<OrganizationsClaims>();

                foreach (var orgcode in orgcodes)
                {
                    var orgInfo = await _organizationRepository.GetOrganizationByIdAsync(orgcode);
                    if (orgInfo!=null)
                    {
                        var orgEmail = orgInfo.OrganizationEmail;
                        var appcodes = await _applicationService.GetAppCodeByOrgCodeAsync(orgcode);
                        var organizationsClaim = new OrganizationsClaims
                        {
                            orgcode = orgcode,
                            applications = new List<AppcodeClaim>(),
                            permissions = orgPermissions,
                            organizationemail = orgEmail
                        };

                        foreach (var appcode in appcodes)
                        {
                            AppcodeClaim appCodeClaim = new AppcodeClaim();
                            var appOwnerEmail = await _applicationRepository.GetApplicationDetailsAsync(appcode);
                            if (appOwnerEmail.IsActive == true)
                            {
                                appCodeClaim.appcode = appcode;
                                appCodeClaim.applicationemail = appOwnerEmail.ApplicationEmail;
                                organizationsClaim.applications.Add(appCodeClaim);
                            }
                        }

                        orgClaims.Add(organizationsClaim);

                    }
                }
                claims.Add(new Claim("organizations", "[" + JsonConvert.SerializeObject(orgClaims) + "]", JsonClaimValueTypes.JsonArray));

            }

            if (!roleids.Contains(1))
            {
                claims.Add(new Claim("permissions", "[]", JsonClaimValueTypes.JsonArray));
            }
            if (!roleids.Contains(2))
            {
                claims.Add(new Claim("organizations", "[]", JsonClaimValueTypes.JsonArray));
            }
            if (!roleids.Contains(3))
            {
                claims.Add(new Claim("applications", "[]", JsonClaimValueTypes.JsonArray));
            }
            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("ValidIssuer"),
                Environment.GetEnvironmentVariable("ValidIssuer"),
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            
            var encryptedJwt = EncryptJwt(token);

            return encryptedJwt;


        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public string EncryptJwt(JwtSecurityToken token)
        {
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var payloadJson = JsonConvert.SerializeObject(tokenString);
            var certificatePath = Environment.GetEnvironmentVariable("CertificatePath");

            X509Certificate2 certWithPublicKey = new X509Certificate2(certificatePath);
            RSA rsaPublicKey = certWithPublicKey.GetRSAPublicKey();

            string encryptedJwt = JWT.Encode(payloadJson, rsaPublicKey, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);

            return encryptedJwt;
        }

        public async Task<string> GenerateF1AdminJweToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("Subject")),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };
            var NullList = new List<Claim>();
            claims.Add(new Claim("email", email));
            claims.Add(new Claim("permission", "race::manage"));
            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("ValidIssuer"),
                Environment.GetEnvironmentVariable("ValidIssuer"),
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );


            var encryptedJwt = EncryptJwt(token);

            return encryptedJwt;
        }

    }
}
