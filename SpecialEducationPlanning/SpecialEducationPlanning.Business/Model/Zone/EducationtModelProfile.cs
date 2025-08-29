using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AiepModelProfile : BaseProfile<AiepModel, Aiep, int>
    {
        protected override IMappingExpression<Aiep, AiepModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.Area, opt => opt.Ignore())
                .ForMember(v => v.BuilderEducationerAieps, opt => opt.Ignore())
                .ForMember(v => v.Educationers, opt => opt.Ignore());
        }

        protected override IMappingExpression<AiepModel, Aiep> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(v => v.Area, opt => opt.Ignore())
                .ForMember(v => v.BuilderEducationerAieps, opt => opt.Ignore());
        }
    }
}

