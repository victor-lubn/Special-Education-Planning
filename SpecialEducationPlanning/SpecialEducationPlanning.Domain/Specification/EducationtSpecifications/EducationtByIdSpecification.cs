using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AiepSpecifications
{
    public class AiepByIdSpecification : Specification<Aiep>
    {
        public AiepByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}

