using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreateAt { get; set; }

    public DateOnly UpdateAt { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
