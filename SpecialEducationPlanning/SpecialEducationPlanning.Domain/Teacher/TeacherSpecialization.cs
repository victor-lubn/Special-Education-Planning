
using SpecialEducationPlanning.Domain.Common;

namespace SpecialEducationPlanning.Domain.Teacher
{
    public class TeacherSpecialization : AuditableEntity
    {
        public int Id { get; set; }
        public string Specialization { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
