
using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning.Api.DtoModel.Teacher
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Qualifications { get; set; }
        public TeacherAddressDto Address { get; set; }
        public ICollection<TeacherCertificationDto> Certifications { get; set; }
        public ICollection<TeacherAvailabilityDto> Availabilities { get; set; }
        public ICollection<TeacherSpecializationDto> Specializations { get; set; }
        public ICollection<TeacherOfficeHourDto> OfficeHours { get; set; }
    }
}
