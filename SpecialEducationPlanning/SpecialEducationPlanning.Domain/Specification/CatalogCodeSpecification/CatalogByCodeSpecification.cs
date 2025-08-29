using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CatalogCodeSpecification
{
    public class CatalogByCodeSpecification : Specification<Catalog>
    {
        public CatalogByCodeSpecification(string code, string EducationOrigin) : base(x =>
            x.Code.Equals(code) && x.EducationToolOrigin.Name == EducationOrigin)
        {
        }
    }
}

