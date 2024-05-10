using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class PracticeResult
{
    public int PracticeResultId { get; set; }

    public int RaceId { get; set; }

    public int DriverId { get; set; }

    public int TeamId { get; set; }

    public string PracticeType { get; set; }

    public int? Position { get; set; }

    public int? LapsCompleted { get; set; }

    public TimeSpan? FastestLapTime { get; set; }

    public virtual Driver Driver { get; set; }

    public virtual Race Race { get; set; }

    public virtual Team Team { get; set; }
}
