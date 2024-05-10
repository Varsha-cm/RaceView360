using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class RaceResult
{
    public int ResultId { get; set; }

    public int RaceId { get; set; }

    public int DriverId { get; set; }

    public int TeamId { get; set; }

    public int Position { get; set; }

    public int? LapsCompleted { get; set; }

    public int? GridPosition { get; set; }

    public TimeSpan? FinishingTime { get; set; }

    public TimeSpan? FastestLapTime { get; set; }

    public decimal? PointsEarned { get; set; }

    public virtual Driver Driver { get; set; }

    public virtual Race Race { get; set; }

    public virtual Team Team { get; set; }
}
