using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CountrySpecifications
{
    public class CountryByNameSpecification : Specification<Country>
    {
        public CountryByNameSpecification(string name) : base(x => x.KeyName == name)
        {
        }
    }
}
