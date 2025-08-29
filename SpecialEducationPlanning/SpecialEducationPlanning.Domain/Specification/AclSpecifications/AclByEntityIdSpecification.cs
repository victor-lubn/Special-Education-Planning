using Koa.Domain.Specification;
using System;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Specification.AreaSpecifications
{
    public class AclByEntityIdSpecification : Specification<Acl>
    {
        public AclByEntityIdSpecification(Type entityType, int entityId) : base(x => x.EntityId == entityId && x.EntityType == entityType.Name)
        {
        }
    }
}