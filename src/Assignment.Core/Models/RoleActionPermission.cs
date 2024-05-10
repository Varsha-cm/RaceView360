using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class RoleActionPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OperationId { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }

        [Display(Name = "Actions")]
        public virtual int ActionId { get; set; }
        [ForeignKey("ActionId")]
        public virtual Actions Actions { get; set; }
        [Display(Name = "Permissions")]
        public virtual int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permissions Permissions { get; set; }
        [Display(Name = "Roles")]
        public virtual int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Roles Roles { get; set; }
        

        
    }
}
