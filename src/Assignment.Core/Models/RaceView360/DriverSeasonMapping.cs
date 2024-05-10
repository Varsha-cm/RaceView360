using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class DriverSeasonMapping
{
    public int DriverSeasonMapping1 { get; set; }

    public int DriverId { get; set; }

    public int TeamId { get; set; }

    public bool IsActive { get; set; }
    public int SeasonId {  get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Driver Driver { get; set; }
    public virtual Season Season { get; set; }

    public virtual Team Team { get; set; }
}
