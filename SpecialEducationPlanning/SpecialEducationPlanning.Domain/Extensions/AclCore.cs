using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Specification;
using SpecialEducationPlanning
.Domain.Specification.AreaSpecifications;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class AclCore
    {

        #region Methods Public

        public static bool BuilderAiepAcl(this DbContext dataContext,
            int builderId, int AiepId)
        {
            dataContext.StoreProcedureAclBuilderAiep(builderId, AiepId);

            return true;
        }

        public static bool BuilderUpdateAcl(this DbContext dataContext, int builderId)
        {
            dataContext.StoreProcedureAclBuilderAieps(builderId);

            return true;
        }

        public static bool AiepUpdateAcl(this DbContext dataContext, int AiepId)
        {
            dataContext.StoreProcedureAclAiep(AiepId);

            return true;
        }

        public static bool EducationerUpdateAcl(this DbContext dataContext, int EducationerId)
        {
            dataContext.StoreProcedureAclEducationer(EducationerId);

            return true;
        }

        public static bool EndUserAiepAcl(this DbContext dataContext,
            int endUserId, int AiepId)
        {
            //TODO ARE THESE IN USE?
            throw new NotImplementedException("EndUserAiepAcl");
        }

        public static bool EndUserUpdateAcl(this DbContext dataContext, int endUserId)
        {
            //TODO ARE THESE IN USE?
            throw new NotImplementedException("EndUserUpdateAcl");
        }

        public static bool PlanUpdateAcl(this DbContext dataContext,
            int planId)
        {
            var planEntity = dataContext.Set<Plan>().Where(new EntityByIdSpecification<Plan>(planId))
                .IgnoreQueryFilters().FirstOrDefault();

            if (planEntity == null)
            {
                return false;
            }

            dataContext.StoreProcedureAclProjectAiep(planEntity.ProjectId);

            return true;
        }

        public static void EndUserAddAcl(this DbContext dbContext, int endUserId)
        {
            dbContext.StoreProcedureAclEndUsersCreate(endUserId);
        }

        public static void ProjectAddAcl(this DbContext dbContext, int projectId)
        {
            dbContext.StoreProcedureAclProjectsCreate(projectId);
        }

        public static void PlanAddAcl(this DbContext dbContext, int planId)
        {
            dbContext.StoreProcedureAclPlansCreate(planId);
        }

        public static void VersionAddAcl(this DbContext dbContext, int versionId)
        {
            dbContext.StoreProcedureAclVersionsCreate(versionId);
        }

        public static void ProjectUpdateAcl(this DbContext dataContext,
            int projectId)
        {
            dataContext.StoreProcedureAclProjectAiep(projectId);

        }

        public async static Task RemoveAllUserAclAsync<T>(this DbContext dataContext, int userId)
        {
            var toRemove = await dataContext.Set<Acl>().Where(new AclByUserIdSpecification(typeof(T), userId)).ToListAsync();
            dataContext.Set<Acl>().RemoveRange(toRemove);
        }

        public async static Task RemoveAllEntityAclAsync<T>(this DbContext dataContext, int entityId)
        {
            var toRemove = await dataContext.Set<Acl>().Where(new AclByEntityIdSpecification(typeof(T), entityId)).ToListAsync();
            dataContext.Set<Acl>().RemoveRange(toRemove);
        }

        #endregion

    }

}

