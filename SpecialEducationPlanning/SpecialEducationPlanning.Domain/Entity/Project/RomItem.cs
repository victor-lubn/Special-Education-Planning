using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class RomItem : BaseEntity<int>
    {
        public string ItemName { get; set; }
        public string SerialNumber { get; set; }
        public string Sku { get; set; }
        public string Range { get; set; }
        public string Colour { get; set; }
        public int Qty { get; set; }
        
        public string Description { get; set; }
        public string Handing {  get; set; }
        public string PosNumber { get; set; }
        public string Annotation { get; set; }
        public string OrderCode { get; set; }

        public int CatalogId { get; set; }
        public virtual Catalog Catalog { get; set; }
        public int VersionId { get; set; }
        public virtual Version Version { get; set; }
    }
}
