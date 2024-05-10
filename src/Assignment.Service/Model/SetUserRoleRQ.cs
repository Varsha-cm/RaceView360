using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class SetUserRoleRQ : BaseRQ
    {
        public int RoleId { get; set; }
        public string? OrgCode { get; set; }
        public string? AppCode { get; set; }
        public string Email { get; set; }
    }
}
