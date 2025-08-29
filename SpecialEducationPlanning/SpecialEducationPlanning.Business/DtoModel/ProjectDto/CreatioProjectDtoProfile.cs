using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class CreatioProjectDtoProfile : Profile
    {
        public CreatioProjectDtoProfile() 
        {
            CreateMap<BuilderDto, BuilderModel>();
        }
    }
}