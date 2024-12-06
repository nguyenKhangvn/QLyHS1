using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLyHS1.Models
{
    public class ClassroomViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }
        public string TeacherName { get; set; }
        public string GrandLevelName { get; set; }
    }

    public class ClassroomDetailViewModel
    {
        public int TeacherID { get; set; }
        public int GrandLevelID { get; set; }
        public string Name { get; set; }
        public string Room { get; set; }
        public int Quantity { get; set; }

    }

     public class ClassroomDetailToEditViewModel
        {
        public int Id { get; set; }
        public int TeacherID { get; set; }
        public int GrandLevelID { get; set; }
        public string Name { get; set; }
        public string Room { get; set; }
        public int Quantity { get; set; }
        public bool? Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
