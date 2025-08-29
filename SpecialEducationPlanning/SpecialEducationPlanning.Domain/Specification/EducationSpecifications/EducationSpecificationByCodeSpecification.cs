using Koa.Domain.Specification;
using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.HousingSpecifications
{
    public class HousingSpecificationByCodeSpecification : Specification<HousingSpecification>
    {
        public HousingSpecificationByCodeSpecification(List<string> codes) : base(x => codes.Contains(x.Code))
        {
        }
    }
}
