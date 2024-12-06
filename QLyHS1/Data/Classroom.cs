using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Classroom
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int GrandLevelId { get; set; }

    public string Name { get; set; } = null!;
    public string Room { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public bool? Status { get; set; }

    public virtual GrandLevel GrandLevel { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Teacher Teacher { get; set; } = null!;
}
