using Koa.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SpecialEducationPlanning
.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class RomItemModel : BaseModel<int>
    {
        [Required]
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(ItemName))]
        public string ItemName { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string SerialNumber { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        public string Sku { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(Range))]
        public string Range { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(Colour))]
        public string Colour { get; set; }
        [JsonPropertyName(nameof(Qty))]
        [JsonConverter(typeof(IntToStringConverter))]
        public int Qty { get; set; }
        
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(Description))]
        public string Description { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(Handing))]
        public string Handing { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(PosNumber))]
        public string PosNumber { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(Annotation))]
        public string Annotation { get; set; }
        [StringLength(DataContext.DefaultPropertyLength)]
        [JsonPropertyName(nameof(OrderCode))]
        public string OrderCode { get; set; }

        public int CatalogId { get; set; }
        public virtual CatalogModel Catalog { get; set; }
        public int VersionId { get; set; }
        public VersionModel Version { get; set; } = new VersionModel();
    }
}
