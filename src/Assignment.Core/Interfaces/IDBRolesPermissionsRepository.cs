using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Interfaces
{
    public interface IDBRolesPermissionsRepository
    {
        public IEnumerable<Roles> GetAllRoles();
        public IEnumerable<PermissionWithAction> GetPermissionsByRole(int roleId);
        public Task<int> GetUserRoleAsync(string email);
        public Task<IEnumerable<PermissionWithAction>> GetUserPermissionsAsync(int email);
        public Task<List<int>> GetRolesByEmail(string email);

    }
}
