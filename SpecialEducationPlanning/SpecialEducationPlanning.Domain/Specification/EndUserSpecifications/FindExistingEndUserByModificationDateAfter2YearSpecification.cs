using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications
{
    public class FindExistingEndUserByModificationDateAfter2YearSpecification : Specification<EndUser>
    {
        public FindExistingEndUserByModificationDateAfter2YearSpecification() :
            base(x => x.UpdatedDate <= (DateTime.UtcNow.AddYears(-2)))
        {
        }
    }
}