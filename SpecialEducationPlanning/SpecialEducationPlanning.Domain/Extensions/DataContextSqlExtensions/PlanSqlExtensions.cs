using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class PlanSqlExtensions
    {
        public static string GetNextPlanCodeSequenceValueAsync(this DbContext context)
        {
            SqlParameter result = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            context.Database.ExecuteSqlRaw(
                       "SELECT @result = (NEXT VALUE FOR PlanCodeSequence)", result);

            int resultInt = (int)result.Value;
            string resultString = resultInt.ToString().PadLeft(5, '0');

            return resultString;
        }

        public static void NullifyMasterVersionSqlAsync(this DbContext context, Plan plan)
        {
            string commandPlanMasterVersion = "UPDATE [Plan] SET MasterVersionId = NULL WHERE MasterVersionId = @param";
            var planMasterVersionId = new SqlParameter("@param", plan.MasterVersionId);

            context.Database.ExecuteSqlRaw(commandPlanMasterVersion, planMasterVersionId);
        }

        public static void DeletePlanByIdSqlAsync(this DbContext context, int planId)
        {
            string command = "DELETE FROM [Plan] WHERE Id = @planIdParam";
            var planIdParam = new SqlParameter("@planIdParam", planId);

            context.Database.ExecuteSqlRaw(command, planIdParam);
        }
    }

}