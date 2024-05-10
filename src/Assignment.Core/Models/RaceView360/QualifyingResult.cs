using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class QualifyingResult
{
    public int QualifyId { get; set; }

    public int RaceId { get; set; }

    public int DriverId { get; set; }

    public int TeamId { get; set; }

    public int? Position { get; set; }

    public TimeSpan? Q1 { get; set; }

    public TimeSpan? Q2 { get; set; }

    public TimeSpan? Q3 { get; set; }

    public int? LapsCompleted { get; set; }

    public virtual Driver Driver { get; set; }

    public virtual Race Race { get; set; }

    public virtual Team Team { get; set; }
}
