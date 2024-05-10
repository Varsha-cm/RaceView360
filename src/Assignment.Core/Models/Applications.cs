using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class Applications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [MaxLength(100)]
        public int ApplicationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApplicationName { get; set; }

        [Required]
        [MaxLength(12)]
        public string AppCode { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public string Phone { get; set; }

        [Required]
        [Display(Name = "Organizations")]
        public virtual int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organizations Organizations { get; set; }

        public bool IsActive { get; set; } = true;

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ApplicationEmail { get; set; }

    }
}
