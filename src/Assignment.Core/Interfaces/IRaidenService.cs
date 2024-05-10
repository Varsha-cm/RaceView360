using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IRaidenService
    {
        public Task<string> ApplicationAuthentication();
    }
}
