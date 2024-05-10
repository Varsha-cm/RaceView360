using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Models
{
    public class ProductsApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductAppId { get; set; }
        [Display(Name = "Applications")]
        public virtual int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Applications Applications { get; set; }

        [Display(Name = "Products")]
        public virtual int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
        
        public bool IsEnabled { get; set; }

       

    }
}
