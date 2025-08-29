using AutoMapper;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class QuoteItemMigrationProfile : BaseProfile<QuoteItemMigrationModel, RomItem, int>
    {
        protected override IMappingExpression<RomItem, QuoteItemMigrationModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(m => m.TdpId, o => o.MapFrom(e => e.Id));
        }

        protected override IMappingExpression<QuoteItemMigrationModel, RomItem> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.SerialNumber, opt => opt.MapFrom(m => m.SerialNmbr))
                .ForMember(e => e.Sku, opt => opt.MapFrom(m => m.SKU))
                .ForMember(e => e.ItemName, opt => opt.MapFrom(s => "Not avaiable"))
                .ForMember(e => e.Range, opt => opt.MapFrom(m => m.Range))
                .ForMember(e => e.Colour, opt => opt.MapFrom(m => m.Colour))
                .ForMember(e => e.Qty, opt => opt.MapFrom(m => m.Quantity ?? 0));
            //.ForMember(e => e.VersionId, opt => opt.MapFrom(m => m.QuoteTdpId));
        }
    }
}
