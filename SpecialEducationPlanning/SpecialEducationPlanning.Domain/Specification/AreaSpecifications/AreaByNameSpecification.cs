using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AreaSpecifications
{
    public class AreaByNameSpecification : Specification<Area>
    {
        public AreaByNameSpecification(string name) : base(x => x.KeyName == name)
        {
        }
    }
}
