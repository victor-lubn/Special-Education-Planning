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
    public class UserMaterializedUserWithRolesModelPagedValueQuery : MultipleMaterializedPagedValueQuery<User, UserWithRoleModel>
    {
        public UserMaterializedUserWithRolesModelPagedValueQuery(ISpecification<User> specification, ISpecification<UserWithRoleModel> users,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, users, sortDescriptor, pageSize)
        {
        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<UserWithRoleModel> Materialize(IQueryable<User> queryable)
        {
            var materialize = queryable.Select(c => new UserWithRoleModel
            {
                Id = c.Id,
                CurrentAiepId = c.CurrentAiepId,
                FirstName = c.FirstName,
                FullAclAccess = c.FullAclAccess,
                Surname = c.Surname,
                UniqueIdentifier = c.UniqueIdentifier,
                AiepId = c.AiepId,
                UserRoles = c.UserRoles.Select(ur => new UserRoleModel
                {
                    Role = new RoleModel()
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name
                    }
                }).ToList(),
                Aiep = new AiepModel()
                {
                    AiepCode = c.Aiep.AiepCode,
                    Name = c.Aiep.Name
                },               
                Leaver=c.Leaver,
                DelegateToUserId=c.DelegateToUserId
            });

            return materialize;
        }
    }
}
