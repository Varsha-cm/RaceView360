using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model
{
    public class UserRQ : BaseRQ
    {
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(16)]
        public string Password { get; set; }
        public string? appcode { get; set; }
        public string? orgcode { get; set; }
        public int roleId { get; set; }
    }

}
