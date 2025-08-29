using Koa.Domain;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class SortingFiltering : BaseEntity<int>
    {
        public string EntityType { get; set; }
        public string PropertyName { get; set; }
        public string PropertyText { get; set; }
    }
}
