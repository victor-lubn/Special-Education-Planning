using Koa.Domain;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Business.Model;

public class HousingSpecificationModel : BaseModel<int>
{
    public int ProjectId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int PlanState { get; set; }
    public virtual ProjectModel ProjectModel { get; set; }
    public virtual ICollection<HousingTypeModel> HousingTypes { get; set; }
    public virtual ICollection<HousingSpecificationTemplatesModel> HousingSpecificationTemplates { get; set; }
}
