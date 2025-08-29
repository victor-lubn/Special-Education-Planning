using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model
{
    public class CatalogModelProfile : BaseProfile<CatalogModel, Catalog, int>
    {
        protected override IMappingExpression<CatalogModel, Catalog> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(c => c.EducationToolOriginId, opt => opt.Ignore())
                .ForMember(c => c.RomItems, opt => opt.Ignore())
                .ForMember(c => c.Versions, opt => opt.Ignore());
        }
        protected override IMappingExpression<Catalog, CatalogModel> MapEntityToModel()
        {
            return base.MapEntityToModel()
                .ForMember(v => v.EducationOrigin, opt => opt.MapFrom((src, dest) => src.EducationToolOrigin?.Name))
                .ForMember(v => v.Versions, opt => opt.Ignore())
                .ForMember(v => v.RomItems, opt => opt.Ignore());
        }
    }
}

