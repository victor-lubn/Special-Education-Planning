using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CatalogCodeSpecification
{
    public class CodeByCatalogSpecification : Specification<Catalog>
    {
        public CodeByCatalogSpecification(string catalog, string EducationOrigin) : base(x =>
            x.Name.Equals(catalog) && x.EducationToolOrigin.Name == EducationOrigin)
        {
        }
    }
}

