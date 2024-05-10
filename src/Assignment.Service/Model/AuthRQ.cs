using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class AuthRQ : BaseRQ
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RefreshRQ : BaseRQ
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
