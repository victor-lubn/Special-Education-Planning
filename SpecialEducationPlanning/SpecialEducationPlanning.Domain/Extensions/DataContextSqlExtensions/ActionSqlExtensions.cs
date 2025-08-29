using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class ActionSqlExtensions
    {
        public static void DeleteOldActions(this DbContext context, DateTime dateTime, double delete)
        {
            string commandRemoveActions = "Delete TOP (1000) FROM [Action] WHERE Date <= @param";
            var date = new SqlParameter("@param", dateTime.AddDays(-delete));

            context.Database.ExecuteSqlRaw(commandRemoveActions, date);
        }
    }

}