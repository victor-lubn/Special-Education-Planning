using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model
{
    public class VersionModelProfile : BaseProfile<VersionModel, Version, int>
    {
        protected override IMappingExpression<Version, VersionModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.Plan, opt => opt.Ignore())
                .ForMember(v => v.Catalog, opt => opt.Ignore())
                .ForMember(v => v.RomItems, opt => opt.Ignore())
                .ForMember(v => v.FittersPackStatus, opt => opt.MapFrom((src, dest) => src.FittersPackStatus?.Name));
        }

        protected override IMappingExpression<VersionModel, Version> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(v => v.Plan, opt => opt.Ignore())
                .ForMember(v => v.Catalog, opt => opt.Ignore());
        }
    }
}