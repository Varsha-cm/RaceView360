using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class OrganizationRQ : BaseRQ
    {
        [Required]
        public string orgcode { get; set; }
        [Required]
        [MaxLength(100)]
        public string OrganizationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string OrganizationEmail { get; set;}

        [Required]
        public string OrganizationPhone { get; set;}        
    }
}
