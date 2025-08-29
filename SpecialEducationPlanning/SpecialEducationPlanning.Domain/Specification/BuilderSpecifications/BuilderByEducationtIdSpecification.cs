using Koa.Domain.Specification;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.BuilderEducationerAiepSpecifications
{
    public class BuilderByAiepIdSpecification : Specification<Builder> 
    {
        public BuilderByAiepIdSpecification(int AiepId) :
            base(x =>
                x.BuilderEducationerAieps.Any(d => d.AiepId == AiepId))
        {
        }
    }
}


