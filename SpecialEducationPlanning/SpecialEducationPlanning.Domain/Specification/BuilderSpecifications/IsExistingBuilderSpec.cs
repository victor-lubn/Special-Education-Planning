using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CustomerSpecifications
{
    public class IsExistingBuilderSpec : Specification<Builder>
    {
        public IsExistingBuilderSpec(string tradingName, string postcode, string address1, string accountNumber) :
            base(x =>
                x.TradingName.Equals(tradingName) &&
                x.Postcode.Equals(postcode) &&
                x.Address1.Equals(address1) &&
                x.AccountNumber.Equals(accountNumber)
            )
        {
        }
    }
}