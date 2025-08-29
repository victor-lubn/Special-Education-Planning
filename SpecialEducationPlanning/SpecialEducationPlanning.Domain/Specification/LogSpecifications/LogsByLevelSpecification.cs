using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.LogSpecifications
{
    public class LogsByLevelSpecification : Specification<Log>
    {
        public LogsByLevelSpecification(string level) :
            base(x => x.Level == level)
        {
        }
    }
}