
namespace SpecialEducationPlanning.Api.DtoModel.Teacher.Analytics
{
    public class TeacherAnalyticsDto
    {
        public int TeacherId { get; set; }
        public int StudentsCount { get; set; }
        public double AverageStudentPerformance { get; set; }
        public int InterventionsCount { get; set; }
    }
}
