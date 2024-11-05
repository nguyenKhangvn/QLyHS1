using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Semester
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreateAt { get; set; }

    public DateOnly UpdateAt { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
