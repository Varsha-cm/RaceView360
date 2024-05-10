using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public  class AppUserRQ:BaseRQ
    {      
        [Required]
        public string FirstName { get;set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress{ get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(16)]
        public string Password { get; set; }           
        
    }
}
