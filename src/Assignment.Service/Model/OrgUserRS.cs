using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public  class OrgUserRS:BaseRS
    {
        public int userId { get; set; }       
        public string OrgCode { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }       

    }
}
