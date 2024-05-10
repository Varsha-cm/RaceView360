using Microsoft.EntityFrameworkCore;
using Assignment.Api.Models;
using Assignment.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository
{
    public class RolesPermissionsRepository : IDBRolesPermissionsRepository
    {

        private readonly RaidenDBContext _dbContext;

        public RolesPermissionsRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Roles> GetAllRoles()
        {
                return _dbContext.Roles.ToList();   
        }

        public IEnumerable<PermissionWithAction> GetPermissionsByRole(int roleId)
        {
                return _dbContext.RoleActionPermission
                    .Where(rap => rap.Roles.RoleId == roleId)
                    .Include(rap => rap.Roles)
                    .Include(rap => rap.Permissions)
                    .Include(rap => rap.Actions)
                    .Select(rap => new PermissionWithAction
                    {
                        ActionName = rap.Actions.ActionName,
                        PermissionName = rap.Permissions.PermissionName,
                        PermissionId = rap.Permissions.PermissionId
                    })
                    .Distinct()
                    .ToList();

        }
        public async Task<int> GetUserRoleAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                var role = await _dbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == user.UserId);
                if (role != null)
                {
                    return role.RoleId;
                }
            }
            // Return null or a default role if the user is not found
            return 0;
        }
        public async Task<List<int>> GetRolesByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var userId = user.UserId;
                var userRoles = await _dbContext.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                return userRoles;
            }

            return new List<int>();
        }
        public async Task<IEnumerable<PermissionWithAction>> GetUserPermissionsAsync(int email)
        {
            var output = await _dbContext.RoleActionPermission
                .Where(rap => rap.Roles.RoleId == email)
                .Include(rap => rap.Roles)
                .Include(rap => rap.Permissions)
                .Include(rap => rap.Actions)
                .Select(rap => new PermissionWithAction
                {
                    ActionName = rap.Actions.ActionName,
                    PermissionName = rap.Permissions.PermissionName,
                    PermissionId = rap.Permissions.PermissionId
                })
                .Distinct()
                .ToListAsync();

            return output;
        }

    }
}
