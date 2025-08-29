using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications
{
    public class BuilderEducationerAiepByIdSpecification : Specification<BuilderEducationerAiep>
    {
        public BuilderEducationerAiepByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}


