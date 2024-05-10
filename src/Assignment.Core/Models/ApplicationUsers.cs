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
    public class ApplicationUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppUserId { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
        [Required]
        [Display(Name = "Users")]
        public virtual int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }

        [Display(Name = "Organizations")]
        public virtual int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organizations Organizations { get; set; }


        [Display(Name = "Organizations")]
        public virtual string Orgcode { get; set; }

        [Display(Name = "Applications")]
        public virtual int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Applications Applications { get; set; }
        [Display(Name = "Applications")]
        public virtual string AppCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
