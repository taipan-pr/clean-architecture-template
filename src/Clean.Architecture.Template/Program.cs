using Autofac.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using System;

namespace Clean.Architecture.Template
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
            // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
            // set up successfully.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("Starting up!");

            try
            {
                CreateHostBuilder(args).Build().Run();
                Log.Information("Stopped cleanly");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var environment = context.HostingEnvironment.EnvironmentName;
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

                    var c = config.Build();
                    config.AddEnvironmentVariables();
                    config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
                })
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    if (!string.IsNullOrWhiteSpace(context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]))
                    {
                        loggerConfiguration
                            .WriteTo.ApplicationInsights(new TelemetryConfiguration
                            {
                                InstrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]
                            }, TelemetryConverter.Traces);
                    }

                    if (!string.IsNullOrWhiteSpace(context.Configuration["Seq:HostName"]))
                    {
                        loggerConfiguration
                            .WriteTo.Seq(context.Configuration["Seq:HostName"],
                                apiKey: context.Configuration["Seq:ApiKey"]);
                    }

                    switch (context.Configuration["Serilog:Properties:Environment"])
                    {
                        case "Local":
                            loggerConfiguration
                                .Enrich.WithProperty("Environment", "Local")
                                .WriteTo.Console();
                            break;
                        default:
                            loggerConfiguration
                                .Enrich.WithProperty("Environment", context.Configuration["ASPNETCORE_ENVIRONMENT"]);
                            break;
                    }

                    loggerConfiguration
                        .Enrich.FromLogContext()
                        .Enrich.WithEnvironmentUserName()
                        .Enrich.WithMachineName()
                        .Enrich.WithClientAgent()
                        .Enrich.WithClientIp()
                        .Enrich.WithCorrelationId()
                        .Enrich.WithExceptionDetails()
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
