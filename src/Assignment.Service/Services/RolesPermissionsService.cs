using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Assignment.Api.Models;
using Assignment.Infrastructure.Interfaces;
using Assignment.Service.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services
{
    public class RolesPermissionService
    {
        private readonly IDBRolesPermissionsRepository _rolePermissionRepository;
        private readonly Serilog.Core.Logger logger;

        public RolesPermissionService(IDBRolesPermissionsRepository rolePermissionRepository, Serilog.Core.Logger logger)
        {
            _rolePermissionRepository = rolePermissionRepository;
            this.logger = logger;

        }

        public IEnumerable<ListRolesRS> GetAllRoles()
        {
            var roles = _rolePermissionRepository.GetAllRoles();
            if (roles == null)
            {
                logger.Information("No records found");
                throw new InvalidOperationException("No records found.");
            }
            var rs = roles.Select(roles => new ListRolesRS
            {
                RoleId = roles.RoleId,
                RoleName = roles.RoleName,
            }).ToList();
            return rs;
        }
        public IEnumerable<ListPermissionsRS> GetPermissionsByRole(int roleId)
        {
            var permissions = _rolePermissionRepository.GetPermissionsByRole(roleId);
            if (!permissions.Any())
            {
                logger.Error($"{roleId} has no permissions or roleId is invalid");
                throw new KeyNotFoundException($"{roleId} has no permissions or roleId is invalid");
            }
            var groupedPermissions = permissions.GroupBy(permission => permission.ActionName);
            var rs = groupedPermissions.Select(group => new ListPermissionsRS
            {
                RoleId = roleId,
                ActionName = group.Key,
                Permissions = group.Select(permission => new Permission
                {
                    PermissionName = permission.PermissionName,
                    PermissionId = permission.PermissionId
                }).ToList()
            }).Distinct().ToList();
            return rs;
        }
        public async Task<int> GetUserRoleAsync(string email)
        {

            var ret = await _rolePermissionRepository.GetUserRoleAsync(email);
            return ret;
        }
        public async Task<List<int>>GetRolesByEmail(string email)
        {

            var ret = await _rolePermissionRepository.GetRolesByEmail(email);
            return ret;
        }
        public async Task<IEnumerable<PermissionWithAction>> GetUserPermissionsAsync(int email)
        {

            var ret = await _rolePermissionRepository.GetUserPermissionsAsync(email);
            return ret;
        }
    }
}

