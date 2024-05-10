using System;
using System.Collections.Generic;

namespace Assignment.Api.Models;

public partial class Circuit
{
    public int CircuitId { get; set; }

    public string CircuitCode { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public string Country { get; set; }

    public decimal Length { get; set; }

    public int Laps { get; set; }

    public decimal RaceDistance { get; set; }

    public string Url { get; set; }

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
