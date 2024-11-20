using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Assignment
{
    public int Id { get; set; }
    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
