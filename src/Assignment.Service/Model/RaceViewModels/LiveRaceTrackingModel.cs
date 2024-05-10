using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class LiveRaceTrackingModel
    {
        public int Position { get;set; }
        public string DriverName { get;set; }
        public string TeamName {  get;set; }
        public string FastestLapTime { get;set; }
        public string LapNumber { get;set; }

    }
    public class DriverLapInfo
    {
        public int DriverID { get; set; }
        public int TotalLapsCompleted { get; set; }
        public TimeSpan FastestLapTime { get; set; }
    }
    public class UpdateLiveRaceRQ
    {
        public int CarNumber { get; set; }
        public TimeOnly LapTime { get; set; }
    }
}
