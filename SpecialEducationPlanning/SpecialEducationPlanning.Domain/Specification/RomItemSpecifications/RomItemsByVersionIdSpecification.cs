using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.RomItemSpecifications
{
    public class RomItemsByVersionIdSpecification : Specification<RomItem>
    {
        public RomItemsByVersionIdSpecification(int versionId) : base(x => x.VersionId == versionId)
        {
        }
    }
}
