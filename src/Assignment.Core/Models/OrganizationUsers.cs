using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assignment.Api.Models
{
    public class OrganizationUsers
    {      
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrgUserId { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
        [Required]
        [Display(Name = "Users")]
        public virtual int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }
        [Required]
        [Display(Name = "Organizations")]
        public virtual int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organizations Organizations { get; set; }

        [Required]
        [Display(Name = "Organizations")]
        public virtual string Orgcode { get; set; }
       

        public bool IsActive { get; set; } = true;

    }
}
      
