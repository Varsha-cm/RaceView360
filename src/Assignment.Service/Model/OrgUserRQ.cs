using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class OrgUserRQ : BaseRQ
    {
        public string OrgCode { get; set; }
        public int UserId { get; set; }
    }
}
