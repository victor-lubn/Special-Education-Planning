using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications
{
    public class BuilderEducationerAiepByBuilderIdSpecification : Specification<BuilderEducationerAiep>
    {
        public BuilderEducationerAiepByBuilderIdSpecification(int builderId) :
            base(x =>
                x.BuilderId.Equals(builderId))
        {
        }
    }
}


