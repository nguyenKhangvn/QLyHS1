using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Assignment
{
    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
