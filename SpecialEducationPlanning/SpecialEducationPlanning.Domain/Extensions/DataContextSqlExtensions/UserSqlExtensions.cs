using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Domain.Extensions.DataContextSqlExtensions
{
    public static class UserSqlExtensions
    {
        public static void StoreProcedureDeleteLeaversAndUpdatePlans(this DbContext dataContext)
        {           
            dataContext.Database.ExecuteSqlRaw("EXEC DeleteLeaversAndUpdatePlans");
        }
    }
}
