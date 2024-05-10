using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class UserRS:BaseRS
    {
        public int UserId { get; set; }
        public string OrgCode { get; set; }
        public string AppCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }

}
