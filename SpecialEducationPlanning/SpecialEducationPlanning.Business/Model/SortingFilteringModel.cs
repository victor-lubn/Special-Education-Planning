using Koa.Domain;

namespace SpecialEducationPlanning
.Business.Model
{
    public class SortingFilteringModel : BaseModel<int>
    {
        public string EntityType { get; set; }
        public string PropertyName { get; set; }
        public string PropertyText { get; set; }
    }
}
