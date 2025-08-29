using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class VersionSqlExtensions
    {
        public static string GetNextExternalIdSequenceValueAsync(this DbContext dataContext)
        {
            SqlParameter result = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            dataContext.Database.ExecuteSqlRaw(
                       "SELECT @result = (NEXT VALUE FOR ExternalIdSequence)", result);

            int resultInt = (int)result.Value;
            return resultInt.ToString().PadLeft(8, '0');
        }

        public static void DeleteVersionByIdSqlAsync(this DbContext context, int versionId)
        {
            string commandVersion = "DELETE FROM [Version] WHERE Id = @param";
            var planIdParamVersion = new SqlParameter("@param", versionId);

            context.Database.ExecuteSqlRaw(commandVersion, planIdParamVersion);
        }
    }

}