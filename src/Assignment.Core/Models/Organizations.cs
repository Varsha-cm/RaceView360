using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Data;

namespace Assignment.Api.Models
{
    public class Organizations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }
        [Required]
        [MaxLength(100)]
        public string OrganizationName { get; set; }
        [MaxLength(100)]
        [Required]
        [Unicode]
        public string OrganizationEmail { get; set; }
        [Required]
        public string OrganizationPhone { get; set; }
        [Required]
        [MaxLength(12)]
        [MinLength(4)]        
        public string OrgCode { get; set; } 
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
        public bool IsActive { get; set; } = true;
    }
}