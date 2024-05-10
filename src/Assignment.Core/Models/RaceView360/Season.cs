using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Season
{
    public int SeasonId { get; set; }

    public int Year { get; set; }

    public string Url { get; set; }
    public virtual ICollection<DriverSeasonMapping> DriverSeasonMappings { get; set; } = new List<DriverSeasonMapping>();

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
