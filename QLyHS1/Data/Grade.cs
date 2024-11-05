using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Grade
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SemesterId { get; set; }

    public int SubjectId { get; set; }

    public int SchoolYearId { get; set; }

    public double GradeI { get; set; }

    public double GradeIi { get; set; }

    public double GradeSemester { get; set; }

    public DateOnly CreateAt { get; set; }

    public DateOnly UpdateAt { get; set; }

    public bool Status { get; set; }

    public virtual SchoolYear SchoolYear { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
