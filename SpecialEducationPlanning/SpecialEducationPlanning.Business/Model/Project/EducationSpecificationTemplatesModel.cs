using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model;

public class HousingSpecificationTemplatesModel : BaseModel<int>
{
    public int HousingSpecificationId { get; set; }
    public int PlanId { get; set; }
    public HousingSpecificationModel HousingSpecificationModel { get; set; }
    public PlanModel Plan { get; set; }
}
