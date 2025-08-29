using Koa.Domain;
using Koa.Domain.Specification;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class EntityByIdSpecification<T> : Specification<T> where T : IObject<int>
    {
        public EntityByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}