using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model
{
    public class RomItemModelProfile : BaseProfile<RomItemModel, RomItem, int>
    {
        protected override IMappingExpression<RomItem, RomItemModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(r => r.Catalog, opt => opt.Ignore());
        }
        protected override IMappingExpression<RomItemModel, RomItem> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(v => v.Catalog, opt => opt.Ignore());
        }
    }
}
