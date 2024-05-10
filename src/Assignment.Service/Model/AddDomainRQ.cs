using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment.Service.Model
{
    public class AddDomainRQ : BaseRQ
    {
        [Required]
        [MaxLength(100)]
        public string Domain { get; set; }
        public string OrgCode { get; set; }
        public string AppCode { get; set; }
    }
}
