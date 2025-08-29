using AutoMapper;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Model
{
    public class RegionModelProfile : BaseProfile<RegionModel, Region, int>
    {
        protected override IMappingExpression<Region, RegionModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(v => v.Country, opt => opt.Ignore());
        }

        protected override IMappingExpression<RegionModel, Region> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(v => v.Country, opt => opt.Ignore());
        }
    }
}