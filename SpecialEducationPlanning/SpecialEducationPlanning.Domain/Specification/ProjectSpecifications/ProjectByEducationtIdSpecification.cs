using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ProjectSpecifications
{
    public class ProjectByAiepIdSpecification : Specification<Project>
    {
        public ProjectByAiepIdSpecification(int AiepId) : base(x => x.AiepId == AiepId)
        {
        }
    }
}

