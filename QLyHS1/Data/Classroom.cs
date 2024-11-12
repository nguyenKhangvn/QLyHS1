using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Classroom
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int GradeLevelId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public bool? Status { get; set; }

    public virtual GrandLevel GradeLevel { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Teacher Teacher { get; set; } = null!;
}
