using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class ListPermissionsRS : BaseRS
    {
        public int RoleId { get; set; }
        public string ActionName { get; set; }
        public List<Permission> Permissions { get; set; }
    }
    public class Permission : BaseRS
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
    public class ListRolesRS : BaseRS
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
 
    }
}
