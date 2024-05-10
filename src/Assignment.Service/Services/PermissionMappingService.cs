using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assignment.Service.Services
{
    public interface IPermissionMappingService
    {
        string[] GetPermissionsForIdentifier(string identifier);
    }

    public class PermissionMappingService : IPermissionMappingService
    {
        private readonly Dictionary<string, string[]> _permissionMappings = new Dictionary<string, string[]>
    {
        {"app-management", new[] { "appsetting::create", "appsetting::delete" } },
        {"app-permissions-all", new[] { "appsetting::create", "appsetting::delete", "appsetting::view", "appsetting::edit" } },
        {"org-management", new[] { "orgsetting::create", "orgsetting::delete" } },
        {"org-permissions-all", new[] { "orgsetting::create", "orgsetting::delete", "orgsetting::view", "orgsetting::edit" } },
        {"view-roles-permissions", new[] { "user::view" } },
        {"manage-org-users", new[] { "user::create", "user::delete", } },
        {"view-users", new[] { "user::view" } },
        {"manage-app-users", new[] { "user::create", "user::delete", } },
        {"enable-disable-product", new[] { "appsetting::enable", "appsetting::disable" } }
    };

        public string[] GetPermissionsForIdentifier(string identifier)
        {
            return _permissionMappings.TryGetValue(identifier, out var permissions) ? permissions : Array.Empty<string>();
        }
    }
}
