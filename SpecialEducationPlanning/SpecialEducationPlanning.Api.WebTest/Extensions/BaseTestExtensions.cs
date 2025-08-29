using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.WebTest
{
    public static class BaseTestExtensions
    {
        public static async Task ExecuteDbContextAsync(this BaseTest current, Func<DbContext, Task> action)
        {
            await current.ExecuteScopeAsync(sp => action(sp.GetService<DbContext>()));
        }

        public static async Task ExecuteScopeAsync(this BaseTest current, Func<IServiceProvider, Task> action)
        {
            using (var scope = current.Server.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public static async Task<T> WithEntityInTheDatabase<T>(this BaseTest current, T entity)
        {
            await current.ExecuteDbContextAsync(async context =>
            {
                await context.AddAsync(entity);
                await context.SaveChangesAsync();
            });
            return entity;
        }
    }
}
