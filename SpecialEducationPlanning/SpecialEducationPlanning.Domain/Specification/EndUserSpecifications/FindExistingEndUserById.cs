using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.EndUserSpecifications
{
    public class FindExistingEndUserById : Specification<EndUser>
    {
        public FindExistingEndUserById(int id) :
            base(x => x.Id == id)
        {
        }
    }
}