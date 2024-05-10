using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string DriverCode { get; set; }

    public int? DriverNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string Nationality { get; set; }

    public string Url { get; set; }

    public virtual ICollection<DriverSeasonMapping> DriverSeasonMappings { get; set; } = new List<DriverSeasonMapping>();

    public virtual ICollection<DriverStanding> DriverStandings { get; set; } = new List<DriverStanding>();

    public virtual ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();

    public virtual ICollection<PracticeResult> PracticeResults { get; set; } = new List<PracticeResult>();

    public virtual ICollection<QualifyingResult> QualifyingResults { get; set; } = new List<QualifyingResult>();

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
}
