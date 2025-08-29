using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AiepSpecifications
{
    public class AiepByBuilderIdSpecification : Specification<BuilderEducationerAiep>
    {
        public AiepByBuilderIdSpecification(int builderId, int AiepId) : base(b => b.BuilderId == builderId && b.AiepId == AiepId)
        {
        }
    }
}


