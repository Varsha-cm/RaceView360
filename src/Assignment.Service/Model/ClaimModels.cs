using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class PermissionsClaim
    {
        public List<string> Permissions { get; set; }
    }
    public class OrganizationsClaims
    {
        public string orgcode { get; set; }
        public List<string> permissions { get; set; }
        public List<AppcodeClaim> applications { get; set; }
        public string organizationemail { get; set; }
    }
    public class ApplicationsClaim
    {
        public string appcode { get; set; }
        public string orgcode { get; set; }
        public List<string> permissions { get; set; }
        public string applicationemail { get; set; }
        public string organizationemail { get; set; }
    }
    public class ServiceClaim
    {
        public List<string> services { get; set; }
    }
    public class AppcodeClaim
    {
        public string appcode { get; set; }
        public string applicationemail { get; set; }
    }
}
