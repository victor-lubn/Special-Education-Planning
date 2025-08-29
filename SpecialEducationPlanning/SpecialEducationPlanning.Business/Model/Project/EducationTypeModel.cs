using Koa.Domain;
using System.Collections;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Model;

public class HousingTypeModel : BaseModel<int>
{
    public int HousingSpecificationId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? PlanId { get; set; }
    public HousingSpecificationModel HousingSpecificationModel { get; set; }
    public virtual ICollection<PlanModel> Plan { get; set; } = new List<PlanModel>();
}
