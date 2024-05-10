using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class Permissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
    }
}
