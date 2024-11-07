using System;
using System.Collections.Generic;

namespace QLyHS1.Data;

public partial class Teacher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Token { get; set; } = null!;

    public bool Role { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public bool Status { get; set; }
  
    public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();

}
