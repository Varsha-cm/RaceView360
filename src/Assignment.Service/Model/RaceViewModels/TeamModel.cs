using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class TeamModel
    {
        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public string Nationality { get; set; }
        public string Url { get; set; }
    }
    public class TeamDetailsModel
    {
        public string TeamCode { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Url { get; set; }

        public List<string> CurrentDrivers { get; set; }  
    }

}
