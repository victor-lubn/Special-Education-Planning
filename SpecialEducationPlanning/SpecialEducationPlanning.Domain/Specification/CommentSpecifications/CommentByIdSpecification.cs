using Koa.Domain.Specification;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.CommentSpecifications
{
    public class CommentByIdSpecification : Specification<Comment>
    {
        public CommentByIdSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}