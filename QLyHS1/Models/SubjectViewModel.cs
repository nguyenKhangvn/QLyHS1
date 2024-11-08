namespace QLyHS1.Models
{
    public class SubjectViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public bool Status { get; set; }

    }

    public class SubjectDetailViewModel
    {

        public string Name { get; set; } = null!;

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public bool Status { get; set; }

    }
}
