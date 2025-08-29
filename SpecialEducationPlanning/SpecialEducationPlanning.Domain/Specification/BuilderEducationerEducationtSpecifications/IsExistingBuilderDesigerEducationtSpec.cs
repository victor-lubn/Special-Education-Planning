using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications
{
    public class IsExistingBuilderEducationerAiepSpec : Specification<BuilderEducationerAiep>
    {
        public IsExistingBuilderEducationerAiepSpec(int builderId, int AiepId) :
            base(x =>
                x.BuilderId.Equals(builderId) &&
                x.AiepId.Equals(AiepId))
        {
        }
    }
}


