
using SpecialEducationPlanning.Api.DtoModel.Teacher;
using SpecialEducationPlanning.Api.DtoModel.Teacher.Analytics;
using SpecialEducationPlanning.Domain.Teacher;
using SpecialEducationPlanning.Domain.Teacher.Analytics;

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
            CreateMap<TeacherAnalytics, TeacherAnalyticsDto>().ReverseMap();
            CreateMap<TeacherOfficeHour, TeacherOfficeHourDto>().ReverseMap();
        }
    }
}
