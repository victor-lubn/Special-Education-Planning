using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.ReleaseInfoSpecifications
{
    public class ReleaseInfoVersionsSpecification : Specification<ReleaseInfo>
    {
        public ReleaseInfoVersionsSpecification(string version, string fusionVersion) : base(x => x.Version == version && x.FusionVersion == fusionVersion)
        {
        }
    }
}
