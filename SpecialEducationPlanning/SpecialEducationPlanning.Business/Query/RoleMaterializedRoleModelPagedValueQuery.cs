using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Query
{
    public class RoleMaterializedRoleModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Role, RoleModel>
    {
        public RoleMaterializedRoleModelPagedValueQuery(ISpecification<Role> specification, ISpecification<RoleModel> roles,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, roles, sortDescriptor, pageSize)
        {

        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<RoleModel> Materialize(IQueryable<Role> queryable)
        {
            var materialize = queryable.Select(c => new RoleModel
            {
                Id = c.Id,
                Name = c.Name
            });
            return materialize;
        }
    }
}
