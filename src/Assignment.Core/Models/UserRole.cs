using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }

        [Required]
        [Display(Name = "Users")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users User { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Roles Role { get; set; }

        [Display(Name = "Organizations")]
        public int? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organizations Organization { get; set; }

        [Display(Name = "Applications")]
        public int? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public Applications Application { get; set; }
       
        }
    }
