using Microsoft.AspNetCore.Mvc;

namespace QLyHS1.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string SubjectName { get; set; }
    }

    public class TeacherDetailViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }

        public string Name { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        
        [HiddenInput(DisplayValue = false)]
        public string? Token { get; set; }

        [HiddenInput]
        public bool Role { get; set; }

        [HiddenInput]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [HiddenInput]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        public bool Status { get; set; }
    }


}