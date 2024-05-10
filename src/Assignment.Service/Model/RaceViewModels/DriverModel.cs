using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Model.RaceViewModels
{
    public class DriverModel
    {
        public string DriverCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? DriverNumber { get; set; }

        public string Nationality { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Url {  get; set; }   

    }

    public class DriverDetailsModel
    {
        public string DriverCode { get; set; }
        public int? DriverNumber { get; set; }
        public string Name { get; set; }
        public string Nationality {  get; set; }
        public int TotalPoints { get; set; }
        public int RacesParticipated { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

}
