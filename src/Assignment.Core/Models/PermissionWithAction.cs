using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class PermissionWithAction
    {
        public string ActionName { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
}
