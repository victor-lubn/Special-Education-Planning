
using SpecialEducationPlanning.Domain.Common;
using System;

namespace SpecialEducationPlanning.Domain.Teacher
{
    public class TeacherCertification : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Authority { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
