using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class RaceResultModel
    {
        public string GrandPrix { get; set; }
        public DateTime Date { get; set; }
        public string Winner { get; set; }
        public string Car { get; set; }
        public int? Laps { get; set; }
        public TimeSpan? Time { get; set; }

    }
    public class RaceResultResponse
    {
        public int Position { get; set; }
        public int? Number { get; set; }
        public string DriverName { get; set; }
        public string TeamName { get; set; }
        public int? Laps { get; set; }
        public TimeSpan? FastestLapTime { get; set; }
        public TimeSpan? TimeOrRetired { get; set; }
        public int? GridPosition { get; set; }
        public decimal? Points { get; set; }
    }

    public class QualifyResultResponse
    {
        public int? Position { get; set; }
        public int? Number { get; set; }
        public string DriverName { get; set; }
        public string TeamName { get; set; }
        public TimeSpan? Q1 { get; set; }
        public TimeSpan? Q2 { get; set; }
        public TimeSpan? Q3 { get; set; }
        public int? Laps { get; set; }
    }
    public class PracticeResultResponse
    {
        public int? Position { get; set; }
        public int? Number { get; set; }
        public string DriverName { get; set; }
        public string TeamName { get; set; }
        public TimeSpan? Time { get; set; }
        public int? Laps { get; set; }

    }
}
