using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class SeasonModel
    {
        public int Year { get; set; }
        public string Url { get; set; }

    }
    public class DriverTeamMappingModel
    {
        public string DriverCode { get; set;}
        public string TeamCode { get; set;}
        public int Season { get; set;}
    }
}
