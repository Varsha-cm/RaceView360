using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class Actions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ActionName { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }

    }
}
