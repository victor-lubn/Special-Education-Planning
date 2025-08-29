using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.RomItemSpecifications
{
    public class RomItemByNameSpecification : Specification<RomItem>
    {
        public RomItemByNameSpecification(string itemName) : base(x => x.ItemName == itemName)
        {
        }
    }
}
