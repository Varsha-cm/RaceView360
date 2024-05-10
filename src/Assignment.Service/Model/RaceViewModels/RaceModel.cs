using Assignment.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Assignment.Service.Model.RaceViewModels
{
   
    public class RaceModelAll
    {
        public string RaceCode { get; set; }
        public string RaceName { get; set; }
        public string CircuitCode { get; set; }
    }
    public class PreviousRaceResultModel
    {
        public string RaceCode { get; set; }
        public string RaceName { get; set; }
        public string CircuitCode { get; set; }
        public int Year { get; set; }
        public DateTime RaceDateTime { get; set; }
        public DateTime? Practice1DateTime { get; set; }
        public DateTime? Practice2DateTime { get; set; }
        public DateTime? Practice3DateTime { get; set; }
        public DateTime? QualifyDateTime { get; set; }

        public string Position1 { get; set; }
        public string Position2 { get; set; }
        public string Position3 { get; set; }
    }
    public class RaceModel
    {
        public string RaceCode { get; set; }
        public string RaceName { get; set; }
        public string CircuitCode { get; set; }
        public int Year { get; set; }
        public DateTime RaceDateTime { get; set; }
        public DateTime? Practice1DateTime { get; set; }
        public DateTime? Practice2DateTime { get; set; }
        public DateTime? Practice3DateTime { get; set; }
        public DateTime? QualifyDateTime { get; set; }
        public string Url { get; set; }
    }


    public class UpdateRaceModel
    {
        public string RaceName { get; set; }
        public string CircuitCode { get; set; }
        public DateTime RaceDateTime { get; set; }
        public DateTime? Practice1DateTime { get; set; }
        public DateTime? Practice2DateTime { get; set; }
        public DateTime? Practice3DateTime { get; set; }
        public DateTime? QualifyDateTime { get; set; }
        public string Url { get; set; }

    }

}