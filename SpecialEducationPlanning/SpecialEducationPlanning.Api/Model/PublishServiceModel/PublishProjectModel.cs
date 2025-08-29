using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Model.PublishServiceModel
{
    public class PublishProjectModel
    {
        public string CRMProjectId { get; set; }

        public List<HousingSpecificatioTenderPackModel> HousingSpecifications { get; set; } = new();
    }

    public class HousingSpecificatioTenderPackModel
    {
        public string CRMHousingSpecificationId { get; set; }
        public List<HousingTypeTenderPackModel> HousingTypes { get; set; } = new();
    }

    public class HousingTypeTenderPackModel
    {
        public string CRMHousingTypeId { get; set; }
        public List<PlanTenderPackModel> Plans { get; set; } = new();
    }

    public class PlanTenderPackModel
    {
        public string PlanCode { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public int? MasterVersionId { get; set;}
        public string PreviewPath { get; set;}
        public List<RomItemTenderPackModel> RomItems { get; set; } = new();

    }

    public class RomItemTenderPackModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string SerialNumber { get; set; }
        public string Sku { get; set; }
        public string Range { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Handing { get; set; }
        public string PosNumber { get; set; }
        public string Annotation { get; set; }
        public string OrderCode { get; set; }
        public int CatalogId { get; set; }

    }


}
