
using AutoMapper;
using SpecialEducationPlanning.Api.DtoModel.Teacher;
using SpecialEducationPlanning.Domain.Teacher;

namespace SpecialEducationPlanning.Business.Mapper
{
    public class TeacherProfile : Profile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<TeacherAddress, TeacherAddressDto>().ReverseMap();
            CreateMap<TeacherCertification, TeacherCertificationDto>().ReverseMap();
            CreateMap<TeacherAvailability, TeacherAvailabilityDto>().ReverseMap();
            CreateMap<TeacherSpecialization, TeacherSpecializationDto>().ReverseMap();
            CreateMap<CreateTeacherDto, Teacher>();
            CreateMap<UpdateTeacherDto, Teacher>();
        }
    }
}
