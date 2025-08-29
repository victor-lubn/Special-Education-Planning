using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class AclStoredProcedures
    {
        public static void StoreProcedureAclBuilderAiep(this DbContext dataContext, int builderId,
            int AiepId)
        {
            var builderIdSql = new SqlParameter("@BuilderId", builderId);
            var AiepIdSql = new SqlParameter("@AiepId", AiepId);

            dataContext.Database.ExecuteSqlRaw("EXEC AclBuilderAiep @BuilderId, @AiepId",
                builderIdSql,
                AiepIdSql);
        }

        public static void StoreProcedureAclBuilderAieps(this DbContext dataContext, int builderId)
        {
            var builderIdSql = new SqlParameter("@BuilderId", builderId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclBuilderAieps @BuilderId", builderIdSql);
        }

        public static void StoreProcedureAclAiep(this DbContext dataContext, int AiepId)
        {
            var AiepIdSql = new SqlParameter("@AiepId", AiepId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclAiep @AiepId", AiepIdSql);
        }

        public static void StoreProcedureAclEducationer(this DbContext dataContext, int EducationerId)
        {
            var EducationerIdSql = new SqlParameter("@EducationerId", EducationerId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclEducationer @EducationerId", EducationerIdSql);
        }

        public static void StoreProcedureAclProjectAiep(this DbContext dataContext, int projectId)
        {
            var projectIdSql = new SqlParameter("@ProjectId", projectId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclProjectAiep @ProjectId", projectIdSql);
        }

        public static void StoreProcedureAclVersionsCreate(this DbContext dataContext, int versionId)
        {
            var versionIdSql = new SqlParameter("@VersionId", versionId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclVersionCreate @VersionId", versionIdSql);
        }

        public static void StoreProcedureAclPlansCreate(this DbContext dataContext, int planId)
        {
            var planIdSql = new SqlParameter("@PlanId", planId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclPlanCreate @PlanId", planIdSql);
        }

        public static void StoreProcedureAclProjectsCreate(this DbContext dataContext, int projectId)
        {
            var projectIdSql = new SqlParameter("@ProjectId", projectId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclProjectCreate @ProjectId", projectIdSql);
        }

        public static void StoreProcedureAclEndUsersCreate(this DbContext dataContext, int endUserId)
        {
            var endUserIdSql = new SqlParameter("@EndUserId", endUserId);
            dataContext.Database.ExecuteSqlRaw("EXEC AclEndUserCreate @EndUserId", endUserIdSql);
        }
    }

}

