
using System;

namespace SpecialEducationPlanning.Api.DtoModel.Teacher
{
    public class TeacherCertificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Authority { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
