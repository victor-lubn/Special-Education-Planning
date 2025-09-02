
using SpecialEducationPlanning.Api.DtoModel.Teacher;

namespace SpecialEducationPlanning.Api.DtoModel.Teacher
{
    public class UpdateTeacherDto
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
    }
}
