
using SpecialEducationPlanning.Domain.Common;
using System;

namespace SpecialEducationPlanning.Domain.Teacher
{
    public class TeacherOfficeHour : AuditableEntity
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
