using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Race
{
    public int RaceId { get; set; }

    public string RaceCode { get; set; }

    public string RaceName { get; set; }

    public int CircuitId { get; set; }

    public int SeasonId { get; set; }

    public Status Status { get; set; }
    public DateTime RaceDateTime { get; set; }

    public DateTime? Practice1DateTime { get; set; }

    public DateTime? Practice2DateTime { get; set; }

    public DateTime? Practice3DateTime { get; set; }

    public DateTime? QualifyDateTime { get; set; }
    public string Url { get; set; }
    public virtual Circuit Circuit { get; set; }

    public virtual ICollection<DriverStanding> DriverStandings { get; set; } = new List<DriverStanding>();

    public virtual ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();

    public virtual ICollection<PracticeResult> PracticeResults { get; set; } = new List<PracticeResult>();

    public virtual ICollection<QualifyingResult> QualifyingResults { get; set; } = new List<QualifyingResult>();

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();

    public virtual Season Season { get; set; }

    public virtual ICollection<TeamStanding> TeamStandings { get; set; } = new List<TeamStanding>();
}
public enum Status
{
    Upcoming, Inprogress, Completed
}
public enum RoundType
{
    Practice1, Practice2, Practice3, Q1, Q2, Q3, Race
}
public enum Practice
{
    Practice1, Practice2, Practice3
}