using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.DtoModel
{
    public class AreaDtoModelProfile : BaseProfile<AreaDtoModel, Area, int>
    {
        protected override IMappingExpression<AreaDtoModel, Area> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(v => v.Aieps, opt => opt.Ignore());
        }
    }
}
