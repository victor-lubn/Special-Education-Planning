using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AiepModelContractHubProfile : BaseProfile<AiepModelContractHub, Aiep, int>
    {
        protected override IMappingExpression<Aiep, AiepModelContractHub> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.Area, opt => opt.Ignore());
        }

        protected override IMappingExpression<AiepModelContractHub, Aiep> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(v => v.Area, opt => opt.Ignore())
                .ForMember(v => v.BuilderEducationerAieps, opt => opt.Ignore());
        }
    }
}

