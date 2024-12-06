namespace QLyHS1.Models
{
    public class GradeViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string SemesterName {get; set;}
        public string SubjectName {get; set;}
        public int SchoolYearName {get; set;}

        public string className { get; set; }
        public double? GradeI {get; set;}
        public double? GradeII {get; set;}
        public double? GradeIII { get; set; }
        public double? GradeI1 { get; set; }
        public double? GradeAverage { get; set; }
        public string? RangeGrade { get; set; }
    }

    public class GradeDetailViewModel 
    {   public int StudentId { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public int SchoolYearId { get; set; }
        public int ClassNameId { get; set; }
        public double? GradeI { get; set; }
        public double? GradeII { get; set; }
        public double? GradeIII { get; set; }
    }
    public class GradeDetailToEditViewModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SemesterId { get; set; }

        public int SubjectId { get; set; }

        public int SchoolYearId { get; set; }
        public int ClassNameId { get; set; }

        public double? GradeI { get; set; }

        public double? GradeII { get; set; }

        public double? GradeSemester { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public bool? Status { get; set; }
    }

    public class GradeAdd
    {
        public double? GradeI { get; set; }
        public double? GradeII { get; set; }
        public double? GradeIII { get; set; }
    }
}
