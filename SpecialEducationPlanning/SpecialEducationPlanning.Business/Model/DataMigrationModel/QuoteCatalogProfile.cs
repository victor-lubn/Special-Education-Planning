using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class QuoteCatalogProfile : BaseProfile<QuoteMigrationModel, Catalog, int>
    {
        protected override IMappingExpression<QuoteMigrationModel, Catalog> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.Code, opt => opt.MapFrom(s => "CADFile")) // To avoid further problems use "CADFile" as catalogue
                .ForMember(e => e.Enabled, opt => opt.MapFrom(s => false))
                .ForMember(e => e.Name, opt => opt.MapFrom(m => m.Catalogue + " CADFile"))
                .ForMember(e => e.Range, opt => opt.MapFrom(m => m.Range));
        }
    }
}
