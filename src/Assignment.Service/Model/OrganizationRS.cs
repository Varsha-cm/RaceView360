using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class OrgDetailsRS : BaseRS
    {       
        public int OrganizationId { get; set; }
        public string OrgCode { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationEmail { get; set; }
        public string OrganizationPhone { get; set; }        
    }

}
