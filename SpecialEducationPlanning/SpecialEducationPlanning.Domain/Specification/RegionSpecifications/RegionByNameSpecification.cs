using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.RegionSpecifications
{
    public class RegionByNameSpecification : Specification<Region>
    {
        public RegionByNameSpecification(string name) : base(x => x.KeyName == name)
        {
        }
    }
}
