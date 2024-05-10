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
    public class GoogleSignIn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoogleSignInId { get; set; }
        [Required]
        [Display(Name = "Users")]
        public virtual int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }
        public string SigninEmail { get; set; } 
        public bool IsEnabled { get; set; }
        public bool HasAcceptedInvite { get; set; }
    }
}
