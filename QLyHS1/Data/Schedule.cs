﻿using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Schedule
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int ClassRoom { get; set; }

    public int DayOfWeek { get; set; }

    public string Infomation { get; set; }

    public string DayOfWeeks { get; set; }

    public string PeriodStudy { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int TeacherId { get; set; }

  /*  public virtual WeekDay DayOfWeekNavigation { get; set; } = null!;*/

    public virtual Subject Subject { get; set; } = null!;
    public virtual Classroom classroom { get; set; } = null!;

    public virtual Teacher? Teacher { get; set; }
}
