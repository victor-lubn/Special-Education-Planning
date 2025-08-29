using SpecialEducationPlanning
.Api.Host.Options.OAuth2;
using SpecialEducationPlanning
.Api.Host.Options.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using SpecialEducationPlanning
.Api.Middlewares;
using SpecialEducationPlanning
.Api.SOAP;
using SoapCore;
using System.ServiceModel;
using SpecialEducationPlanning
.Api.Middleware.ExceptionMiddleware;
using SpecialEducationPlanning
.Api.Middleware.PermissionMiddleware;
using System;
using System.IO;


namespace SpecialEducationPlanning
.Api.Host
{

    public class Startup
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly string environmentAndCountryName;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            environmentAndCountryName = FindCountryCodeByMachineName(Environment.MachineName.ToLower());

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Email(new EmailConnectionInfo
                {
                    EmailSubject = $"Fault detected with DV Startup.cs in {environmentAndCountryName}",
                    FromEmail = "startup.logger@outlook.com",
                    ToEmail = "ea0ef932.aiep.com@uk.teams.ms",
                    MailServer = "smtp-mail.outlook.com",
                    Port = 25,
                    EnableSsl = false,
                    NetworkCredentials = new System.Net.NetworkCredential("startup.logger@outlook.com", "startupLogger123"),
                }, restrictedToMinimumLevel: LogEventLevel.Fatal)
                  .CreateLogger();

            this.environment = env;
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                if (!this.configuration.GetValue("DistributedCache:OnMemory", false))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = this.configuration.GetValue("DistributedCache:Configuration", string.Empty);
                        options.InstanceName = this.configuration.GetValue("DistributedCache:InstanceName", string.Empty);
                    });
                }
                else
                {
                    services.AddDistributedMemoryCache();
                }

                services.AddApi(this.configuration, opt =>
                {
                    opt.UseSqlServer(this.configuration.GetConnectionString("DefaultConnectionString"));
                });

                services.AddCors();
                services.AddSwagger();
                services.AddOAuth2();


                //services.AddHealthChecks()
                //    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });
                //services.AddHealthChecksUI();
               }
            catch (Exception ex)
            {
                //using the ASP environment varable as it is only set to local when running locally.
                string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (environment is not null && !environment.ToLower().Equals("local", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Fatal(ex, $"A critical fault has occurred in {environmentAndCountryName} while executing the ConfigureServices method in the Startup class");
                    throw;
                }
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime, ILogger<Startup> logger)
        {
            try
            {
                appLifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("ApplicationStopping called.");
                });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }


                if (this.configuration.GetValue("Sap:Authentication:Enabled", true))
                {
                    logger.LogDebug("Adding HttpSapAuthenticationMiddleware");
                    app.UseMiddleware<SapAuthenticationMiddleware>(this.configuration.GetValue<string>("Sap:Authentication:Token"));
                }

                logger.LogDebug("Configuring SAP SOAP services...");

                app.UseSoapEndpoint<ISoapSapService>($"{SoapSapService.UrlPath}.svc", new SoapEncoderOptions());
                app.UseSoapEndpoint<ISoapSapService>($"{SoapSapService.UrlPath}.asmx", new SoapEncoderOptions(),
                    SoapSerializer.XmlSerializer);

                app.UseCors(builder => builder.AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                );

                app.UseExceptionMiddleware();
                //ConfigureJwt Swagger
                app.UseSwagger();
                app.UseSwaggerUI();

                if (this.configuration.GetValue("Middleware:AuditLog:Enabled", true))
                {
                    logger.LogDebug("Configuring AuditLog middleware...");
                    app.UseMiddleware(typeof(AuditLogMiddleware));
                }

                app.UseStaticFiles();
                app.UseSerilogRequestLogging();

                app.UseApi(logger, this.configuration, endpoints =>
                {

                    //endpoints.MapHealthChecks("/healthz/ready", new HealthCheckOptions()
                    //{
                    //    Predicate = (check) => check.Tags.Contains("ready"),
                    //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    //});
                    //endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions()
                    //{
                    //    Predicate = (_) => true,
                    //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    //});

                    //endpoints.MapHealthChecksUI(setup =>
                    //{
                    //    setup.AddCustomStylesheet("wwwroot\\css\\healthz.css");
                    //});
                });
            }
            catch (Exception ex)
            {
                string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment is not null && !environment.ToLower().Equals("local", StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Fatal(ex, $"A critical fault has occurred in {environmentAndCountryName} while executing the Configure method in the Startup class");
                    throw;
                }
            }
        }

        private static string FindCountryCodeByMachineName(string machineName) //The machine name DV is being run off will have roi if it is in ROI and so on, this method uses that to find out where it is being run. 
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
