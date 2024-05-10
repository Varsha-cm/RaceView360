using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class LapTime
{
    public int LapId { get; set; }

    public int RaceId { get; set; }

    public int RoundId { get; set; }

    public RoundType RoundType { get; set; }

    public int Lap { get; set; }

    public int DriverId { get; set; }

    public TimeSpan LapTime1 { get; set; }

    public virtual Driver Driver { get; set; }

    public virtual Race Race { get; set; }

    public virtual Round Round { get; set; }
}
