using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class CircuitModel
    {
        [Required]
        public string CircuitCode { get; set; }
        [Required]
        public string CircuitName { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }  
        public decimal CircuitLength { get; set; }
        public decimal RaceDistance { get; set; }
        public int Laps { get; set; }
        public string? Url { get; set; }
    }

    public class UpdateCircuitRQ
    {
        public string CircuitName { get; set; }
        public decimal CircuitLength { get; set; }
        public decimal RaceDistance { get; set; }
        public int Laps { get; set; }
        public string Url { get; set; } 
    }
}
