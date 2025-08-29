using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model;

public class ProjectTemplatesModel : BaseModel<int>
{
    public int ProjectId { get; set; }
    public int PlanId { get; set; }
    public ProjectModel Project { get; set; }
    public PlanModel Plan { get; set; }
}
