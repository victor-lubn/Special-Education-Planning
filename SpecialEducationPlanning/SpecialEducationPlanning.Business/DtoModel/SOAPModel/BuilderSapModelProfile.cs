using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class BuilderSapModelProfile : BaseProfile<BuilderSapModel, Builder, int>
    {
        protected override IMappingExpression<BuilderSapModel, Builder> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(b => b.Id, opt => opt.Ignore());
        }
    }
}