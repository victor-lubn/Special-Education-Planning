
using System;

namespace SpecialEducationPlanning.Api.DtoModel.Teacher
{
    public class TeacherAvailabilityDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
