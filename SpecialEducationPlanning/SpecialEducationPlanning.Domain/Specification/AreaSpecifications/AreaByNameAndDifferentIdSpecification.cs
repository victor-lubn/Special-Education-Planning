using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AreaSpecifications
{
    public class AreaByNameAndDifferentIdSpecification : Specification<Area>
    {
        public AreaByNameAndDifferentIdSpecification(string name, int id) : base(x => x.KeyName == name && x.Id != id)
        {
        }
    }
}
