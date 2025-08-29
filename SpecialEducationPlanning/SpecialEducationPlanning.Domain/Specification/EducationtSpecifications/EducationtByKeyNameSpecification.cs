using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AiepSpecifications
{
    public class AiepByKeyNameSpecification : Specification<Aiep>
    {
        public AiepByKeyNameSpecification(string code) : base(x => x.AiepCode.Contains(code))
        {
        }
    }
}
