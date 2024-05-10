using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public  class ClientTokenRQ:BaseRQ
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}
