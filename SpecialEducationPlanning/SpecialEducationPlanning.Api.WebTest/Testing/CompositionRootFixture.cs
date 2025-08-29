using Microsoft.Extensions.Configuration;
using Respawn;

namespace SpecialEducationPlanning
.Api.WebTest
{
    public class CompositionRootFixture
    {

        private static Checkpoint CheckPoint = new Checkpoint()
        {
            TablesToIgnore = new[] { "__EFMigrationsHistory" },
        };

        private static IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", false)
                        .AddEnvironmentVariables()
                        .Build();

        public CompositionRootFixture()
        {


        }


        public static void ResetDatabase()
        {
            CheckPoint.Reset(config.GetConnectionString("DefaultConnection")).Wait();
        }


    }
}
