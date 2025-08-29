using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SpecialEducationPlanning
.Domain.Extensions
{

    public static class RomItemSqlExtensions
    {
        public static void DeleteRomItemSqlAsync(this DbContext context, int romItemId)
        {
            string commandRom = "DELETE FROM [RomItem] WHERE Id = @param";
            var planIdParamRom = new SqlParameter("@param", romItemId);

            context.Database.ExecuteSqlRaw(commandRom, planIdParamRom);
        }
    }

}