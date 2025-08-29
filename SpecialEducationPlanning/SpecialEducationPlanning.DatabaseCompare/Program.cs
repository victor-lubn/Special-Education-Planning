using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Education;
using System;
using SpecialEducationPlanning
.Domain;


namespace SpecialEducationPlanning
.DatabaseMigrations
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Create Database");

            var dbcontext = new DbContextFactory().CreateDbContext(args);

            try
            {
                dbcontext.Database.EnsureCreated();
                Console.WriteLine("Create Completed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var exceptionLoop = e.InnerException;
                while (exceptionLoop != null)
                {
                    Console.WriteLine(exceptionLoop);
                    exceptionLoop = exceptionLoop.InnerException;
                }

                Environment.Exit(1);
            }

            Environment.Exit(0);
        }
    }

    public class DbContextFactory : IEducationTimeDbContextFactory<DataContext>
    {
        private const string ConnectionStringLocal =
            "Server=(localdb)\\mssqllocaldb;Database=SpecialEducationPlanning
CodeFirst;Trusted_Connection=True;MultipleActiveResultSets=true";


        public DataContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("TDP_ConnectionString");


            if (connectionString.IsNullOrEmpty())
            {
                connectionString = ConnectionStringLocal;
                Console.WriteLine("Using DefaultConnectionString localdb");
            }
            else
            {
                Console.WriteLine("Using enviroment TDP_ConnectionString");
            }

            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(GetType().Assembly.FullName));
            return new DataContext(builder.Options, null, null, null);
        }
    }
}
