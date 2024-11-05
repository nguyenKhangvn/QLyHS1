using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class GrandLevel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
}
