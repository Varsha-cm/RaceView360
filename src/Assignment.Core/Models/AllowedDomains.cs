using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment.Api.Models
{
    public class AllowedDomains
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllowedDomainId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Domain { get; set; }

        [Display(Name = "Organizations")]
        public virtual int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organizations Organizations { get; set; }

        [Display(Name = "Applications")]
        public virtual int? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Applications Applications { get; set; }
        public bool IsActive { get; set; } 
    }
}
