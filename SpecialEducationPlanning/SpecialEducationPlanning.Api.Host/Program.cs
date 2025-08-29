using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpecialEducationPlanning
.Api.Host
{
    public static class Program
    {
        private const string OutputTemplate =
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}]{NewLine}{Message:lj} {NewLine}{Exception}";
        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.Debug(outputTemplate: OutputTemplate)
                .CreateLogger();
            
#if DEBUG
            AppDomain.CurrentDomain.FirstChanceException += (o, fce) =>
            {
                var message = fce.Exception.Message;
                switch (message)
                {
                    case string a when a.Contains("There is already an object named 'Job' in the database"):
                        //hangfire trying everytime to setup sql tables when the pod restarts. So don't log this.
                        break;
                    case string b when b.Contains("The specified container already exists"):
                        //Storage account create throws even when we use "if not aready exists" method. so don't log this.
                        break;
                    default:
                        Log.Logger.Debug(fce.Exception, "FirstChanceException");
                        break;
                }
            };
#endif

            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                
                if (environment is not null && !environment.ToLower().Equals("local", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Fatal(ex, $"Application start-up failed in {FindCountryCodeByMachineName(Environment.MachineName.ToLower())}");
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HostOptions>(option => { option.ShutdownTimeout = TimeSpan.FromSeconds(30); });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                        .AddJsonFile("secrets/appsettings.secrets.json", true, true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }


                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext();
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerConfiguration
                            .WriteTo.Debug(outputTemplate: OutputTemplate)
                            .WriteTo.Console(outputTemplate: OutputTemplate);
                    }


                }, true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(serverOptions =>
                        {
                            // Tune kestrel options
                            serverOptions.Limits.MaxRequestBodySize = 100_000_000; // 100 mb
                        })
                        .UseStartup<Startup>();
                })
                .UseDefaultServiceProvider((env, c) =>
                {
                    if (env.HostingEnvironment.IsEnvironment("Local"))
                    {
                        c.ValidateScopes = true;
                    }
                });
        }

        private static string FindCountryCodeByMachineName(string machineName)
        {
            string country;
            string environment = FindEnvironmentByMachineName(machineName);

            switch (machineName)
            {
                case string ROI when machineName.Contains("roi"):
                    country = "ROI";
                    break;

                case string france when machineName.Contains("fra"):
                    country = "France";
                    break;

                case string UK when machineName.Contains("tdp"):
                    country = "the UK";
                    break;

                default:
                    country = "Country N/A";
                    break;
            }
            return ($"{country} in the {environment} environment");
        }

        private static string FindEnvironmentByMachineName(string machineName)
        {
            switch (machineName)
            {
                case string Dev when machineName.Contains("dev"):
                    return ("development");
                case string QA when machineName.Contains("qa"):
                    return ("quality assurance");
                case string preprod when machineName.Contains("preprod"):
                    return ("pre-production");
                case string prod when machineName.Contains("prod"):
                    return ("production");
                default:
                    return ($"unable to discern environment, machine name is: {machineName}");
            }
        }
    }
}
