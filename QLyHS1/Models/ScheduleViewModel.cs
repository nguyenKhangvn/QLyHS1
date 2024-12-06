using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLyHS1.Models
{
    public class ScheduleViewModel
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string ClassRoom { get; set; }
        public int DayOfWeek { get; set; }

        public string DayOfWeeks { get; set; }
        public string PeriodStudy { get; set; }
        public string Infomation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class ScheduleDetailViewModel
    {
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }

        public int ClassRoom { get; set; }
        public int DayOfWeek { get; set; }
        public string Infomation { get; set; }
        public string DayOfWeeks { get; set; }
        public string PeriodStudy { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public class ScheduleToEditViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int ClassRoom { get; set; }
        public int DayOfWeek { get; set; }

        public string Infomation { get; set; }
        public string DayOfWeeks { get; set; }
        public string PeriodStudy { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
