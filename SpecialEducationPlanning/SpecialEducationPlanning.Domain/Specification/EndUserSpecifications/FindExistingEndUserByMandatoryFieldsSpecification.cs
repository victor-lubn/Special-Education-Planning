using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications
{
    public class FindExistingEndUserByMandatoryFieldsSpecification : Specification<EndUser>
    {
        public FindExistingEndUserByMandatoryFieldsSpecification(string surname, string postCode, string adress1) :
            base(x =>
                (x.Surname.Equals(surname) &&
                x.Postcode.Equals(postCode) &&
                x.Address1.Equals(adress1))
            )
        {
        }
    }
}