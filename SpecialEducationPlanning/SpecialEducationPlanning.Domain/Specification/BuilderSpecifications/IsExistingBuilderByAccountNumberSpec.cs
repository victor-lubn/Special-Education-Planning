using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CustomerSpecifications
{
    public class IsExistingBuilderByAccountNumberSpec : Specification<Builder>
    {
        public IsExistingBuilderByAccountNumberSpec(string accountNumber) :
            base(x => x.AccountNumber.Equals(accountNumber))
        {
        }
    }
}