using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CountrySpecifications
{
    public class CountryByIdSpecification : Specification<Country>
    {
        public CountryByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}