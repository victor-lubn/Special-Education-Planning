
using SpecialEducationPlanning.Domain.Common;

namespace SpecialEducationPlanning.Domain.Teacher.Analytics
{
    public class TeacherAnalytics : AuditableEntity
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int StudentsCount { get; set; }
        public double AverageStudentPerformance { get; set; }
        public int InterventionsCount { get; set; }
    }
}
