using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CustomerSpecifications
{
    public class BuilderByAccountNumberSpecification : Specification<Builder>
    {
        public BuilderByAccountNumberSpecification(string accountNumber) : base(x => x.AccountNumber == accountNumber)
        {
        }
    }
}