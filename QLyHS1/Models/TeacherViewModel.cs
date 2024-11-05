namespace QLyHS1.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string Subject { get; set; }
        public string StartTime { get; set; }
    }

    public class TeacherDetailViewModel
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

        public string CreateAt { get; set; }

        public string UpdateAt { get; set; }

        public bool Status { get; set; }
    }

}
