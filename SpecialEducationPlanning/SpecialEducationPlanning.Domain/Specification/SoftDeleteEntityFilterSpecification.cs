using Koa.Domain;
using Koa.Domain.Specification;

namespace SpecialEducationPlanning
.Domain.Specification
{
    public class SoftDeleteEntityFilterSpecification<T> : Specification<T> where T : IObject<int>, ISoftDeleteEntity
    {
        public SoftDeleteEntityFilterSpecification() : base(x => !x.IsDeleted)
        {
        }
    }
}