using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class ProductsRQ : BaseRQ
    {
        public string AppCode { get; set; }

        public int ProductId { get; set; }

        public bool IsEnabled { get; set; }
    }
}
