using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class AppDetailsRS : BaseRS
    {
        public int ApplicationId { get; set; }
        public string OrgCode { get; set; }
        public string ApplicationName { get; set; }
        public string AppCode { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApplicationEmail { get; set; }



    }
}
