using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class UpdateApplicationRQ : BaseRQ
    {
        //[Required]
        //public string OrgCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApplicationName { get; set; }

        public string Phone { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }
        public string ApplicationEmail { get; set; }


    }
}
