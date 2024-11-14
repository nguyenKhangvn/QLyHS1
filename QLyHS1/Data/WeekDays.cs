using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class WeekDay
{
    public int Id { get; set; }

    public string? DayName { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
