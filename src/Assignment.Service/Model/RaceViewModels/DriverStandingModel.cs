using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{

    public class DriverStandingModel
    {
        public string GrandPrix { get; set; }
        public DateTime Date { get; set; }
        public string Car { get; set; }
        public int RacePosition { get; set; }
        public int? Points { get; set; }
    }

    public class SeasonDriverStandingModel
    {
        public int Position { get; set; }
        public string DriverName { get; set; }
        public string Nationality { get; set; }
        public string Team { get; set; }
        public int? Points { get; set; }
    }

    public class TeamStandingModel
    {
        public string GrandPrix { get; set; }
        public DateTime Date { get; set; }
        public int? Points { get; set; }
    }


    public class SeasonTeamStandingModel
    {
        public int Position { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
    }
}
