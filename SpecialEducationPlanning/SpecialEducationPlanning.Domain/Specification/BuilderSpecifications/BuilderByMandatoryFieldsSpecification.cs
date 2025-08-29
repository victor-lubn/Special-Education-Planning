using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;


namespace SpecialEducationPlanning
.Domain.Specification.BuilderSpecifications
{
    public class BuilderByMandatoryFieldsSpecification : Specification<Builder>
    {
        public BuilderByMandatoryFieldsSpecification(string tradingName, string postcode, string address1) :
            base(x =>
                x.TradingName.Equals(tradingName) &&
                x.Postcode.Equals(postcode) &&
                x.Address1.Equals(address1)
            )
        {
        }

    }
}
