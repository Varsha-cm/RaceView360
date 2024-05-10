using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Round
{
    public int RoundId { get; set; }

    public int RaceId { get; set; }

    public RoundType RoundType { get; set; }

    public Status RoundStatus { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();

    public virtual Race Race { get; set; }
}
