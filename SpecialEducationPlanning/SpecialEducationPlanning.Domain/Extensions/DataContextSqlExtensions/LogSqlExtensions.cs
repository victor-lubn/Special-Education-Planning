using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class LogSqlExtensions
    {
        public static void DeleteOldLogs(this DbContext dataContext, DateTime dateTime, double delete)
        {
            string commandRemoveLogs = "Delete FROM [Log] WHERE TimeStamp <= @param";
            var date = new SqlParameter("@param", dateTime.AddDays(-delete));

            dataContext.Database.ExecuteSqlRaw(commandRemoveLogs, date);
        }
    }

}