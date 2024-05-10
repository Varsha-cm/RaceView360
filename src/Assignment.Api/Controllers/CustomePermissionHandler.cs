using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using Amazon.Auth.AccessControlPolicy;
using Assignment.Service.Services;

namespace Assignment.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        public CustomAuthorizeAttribute(string identifier)
        {
            _permissions = GetPermissionsForIdentifier(identifier);
        }
        private static string[] GetPermissionsForIdentifier(string identifier)
        {
            var permissionMappingService = new PermissionMappingService();
            return permissionMappingService.GetPermissionsForIdentifier(identifier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<CustomAuthorizationService>();

            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authorizationHeader.Replace("Bearer ", "");

            if (!authService.IsF1AdminAuthorized(token, _permissions).Result)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
