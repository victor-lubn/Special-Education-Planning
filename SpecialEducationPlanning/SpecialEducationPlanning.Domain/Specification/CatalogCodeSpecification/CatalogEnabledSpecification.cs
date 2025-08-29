using SpecialEducationPlanning
.Domain.Entity;
using Koa.Domain.Specification;

namespace SpecialEducationPlanning
.Domain.Specification.CatalogCodeSpecification
{
    public class CatalogEnabledSpecification : Specification<Catalog>
    {
        public CatalogEnabledSpecification(string EducationOrigin) : base(x =>
            x.Enabled && x.EducationToolOrigin.Name == EducationOrigin)
        {
        }
    }
}
