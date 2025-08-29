using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class AreaModelProfile : BaseProfile<AreaModel, Area, int>
    {
        protected override IMappingExpression<Area, AreaModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(v => v.Region, opt => opt.Ignore());
        }

        protected override IMappingExpression<AreaModel, Area> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(v => v.Region, opt => opt.Ignore());
        }
    }
}