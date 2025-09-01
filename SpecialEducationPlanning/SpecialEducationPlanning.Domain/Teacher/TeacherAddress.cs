
using SpecialEducationPlanning.Domain.Common;

namespace SpecialEducationPlanning.Domain.Teacher
{
    public class TeacherAddress : AuditableEntity
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
