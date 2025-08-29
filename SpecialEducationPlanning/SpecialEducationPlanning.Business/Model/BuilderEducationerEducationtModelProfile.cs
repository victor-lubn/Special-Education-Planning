using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model
{
    public class BuilderEducationerAiepModelProfile : BaseProfile<BuilderEducationerAiepModel, BuilderEducationerAiep, int>
    {
        protected override IMappingExpression<BuilderEducationerAiep, BuilderEducationerAiepModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(b => b.Builder, opt => opt.Ignore())
                .ForMember(b => b.Aiep, opt => opt.Ignore());
        }

        protected override IMappingExpression<BuilderEducationerAiepModel, BuilderEducationerAiep> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(b => b.Builder, opt => opt.Ignore())
                .ForMember(b => b.Aiep, opt => opt.Ignore());
        }
    }
}


