﻿using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Schedule
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public string ClassRoom { get; set; } = null!;

    public int DayOfWeek { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public virtual Subject Subject { get; set; } = null!;
}
