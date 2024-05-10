using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamCode { get; set; }

    public string TeamName { get; set; }

    public string Nationality { get; set; }

    public string Url { get; set; }

    public virtual ICollection<DriverSeasonMapping> DriverSeasonMappings { get; set; } = new List<DriverSeasonMapping>();
    public virtual ICollection<DriverStanding> DriverStandings { get; set; } = new List<DriverStanding>();

    public virtual ICollection<PracticeResult> PracticeResults { get; set; } = new List<PracticeResult>();

    public virtual ICollection<QualifyingResult> QualifyingResults { get; set; } = new List<QualifyingResult>();

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();

    public virtual ICollection<TeamStanding> TeamStandings { get; set; } = new List<TeamStanding>();
}
