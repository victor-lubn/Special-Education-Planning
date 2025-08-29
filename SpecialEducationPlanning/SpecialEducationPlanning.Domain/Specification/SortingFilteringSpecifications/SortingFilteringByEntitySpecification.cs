using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class SortingFilteringByEntitySpecification : Specification<SortingFiltering>
    {
        public SortingFilteringByEntitySpecification(string entity) : base(x => x.EntityType.Equals(entity))
        {
        }
    }
}