using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications
{
    public class BuilderEducationerAiepCheckByBuilderIdAiepIdSpecification : Specification<BuilderEducationerAiep>
    {
        public BuilderEducationerAiepCheckByBuilderIdAiepIdSpecification(int builderId, int AiepId) :
            base(x =>
                x.BuilderId.Equals(builderId) &&
                x.AiepId.Equals(AiepId))
        {
        }
    }

}


