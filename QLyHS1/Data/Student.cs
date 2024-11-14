using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Student
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string Name { get; set; } = null!;

    public string? Gender { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string PhoneParent { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateOnly CreateAt { get; set; }

    public DateOnly UpdateAt { get; set; }

    public bool Status { get; set; }

    public string? Conduct { get; set; }

    public virtual Classroom Class { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
