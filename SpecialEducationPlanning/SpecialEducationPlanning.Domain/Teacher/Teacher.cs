
using SpecialEducationPlanning.Domain.Common;
using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning.Domain.Teacher
{
    public class Teacher : AuditableEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Qualifications { get; set; }
        public TeacherAddress Address { get; set; }
        public ICollection<TeacherCertification> Certifications { get; set; }
        public ICollection<TeacherAvailability> Availabilities { get; set; }
        public ICollection<TeacherSpecialization> Specializations { get; set; }
    }
}
