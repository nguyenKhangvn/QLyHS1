using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLyHS1.Data;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime  CreateAt { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime UpdateAt { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
