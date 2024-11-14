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
    public int ClassNameID { get; set; }

    public double GradeI { get; set; }

    public double GradeII { get; set; }

    public double GradeSemester { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public bool? Status { get; set; }

    public virtual SchoolYear SchoolYear { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
    public virtual Classroom Classroom { get; set; }
}
