using Microsoft.AspNetCore.Mvc.Rendering;
using QLyHS1.Data;

namespace QLyHS1.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string ClassName { get; set; }
        public string Address { get; set; }
        public string ParentPhone { get; set; }
    }

    public class StudentDetailViewModel
    {
        public int ClassID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string PhoneParent { get; set; }
        public string Address { get; set; }
    }

    public class StudentDetailToEditViewModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string PhoneParent { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        
    }

}
