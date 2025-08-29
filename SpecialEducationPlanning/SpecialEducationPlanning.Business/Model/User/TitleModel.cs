using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class TitleModel : BaseModel<int>
    {
        public string TitleName { get; set; }
    }
}
