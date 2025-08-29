using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AreaSpecifications
{
    public class AclByUserIdSpecification : Specification<Acl>
    {
        public AclByUserIdSpecification(Type entityType, int userId) : base(x => x.UserId == userId && x.EntityType == entityType.Name)
        {
        }
    }
}