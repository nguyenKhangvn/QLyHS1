using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class SchoolYear
{
    public int Id { get; set; }

    public int Year { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
