using System;
using System.Reflection;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.Discord;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.WebApi.Data.Entity;
using Devi.ServiceHosts.WebApi.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenSearch.Net;

using Serilog;
using Serilog.Sinks.OpenSearch;

namespace Devi.ServiceHosts.WebApi;

/// <summary>
/// Program
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext()
                                                           .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        var openSearchUrl = Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_URL");
        var environment = Environment.GetEnvironmentVariable("DEVI_ENVIRONMENT");

        if (string.IsNullOrEmpty(openSearchUrl) == false
         && string.IsNullOrEmpty(environment) == false)
        {
            Func<ConnectionConfiguration, ConnectionConfiguration> modifyConnectionSettings = null;

            var user = Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_USER");

            if (string.IsNullOrWhiteSpace(user) == false)
            {
                modifyConnectionSettings = obj =>
                                           {
                                               obj.BasicAuthentication(user, Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_PASSWORD"));

                                               // HACK / TODO - Create real certificate
                                               obj.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
                                               return obj;
                                           };
            }

            loggerConfiguration.WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(Environment.GetEnvironmentVariable("DEVI_OPENSEARCH_URL")))
                                                   {
                                                       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.OSv2,
                                                       AutoRegisterTemplate = true,
                                                       MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                                                       IndexFormat = $"devi-{environment}-{DateTime.Now:yyyy-MM}",
                                                       ModifyConnectionSettings = modifyConnectionSettings
                                                   });
        }

        Log.Logger = loggerConfiguration.CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();

        builder.Services.AddTransient<RepositoryFactory>();

        var singletons = builder.Services.AddServices(Assembly.GetExecutingAssembly(),
                                                      Assembly.Load("Devi.Core"),
                                                      Assembly.Load("Devi.ServiceHosts.Core"),
                                                      Assembly.Load("Devi.ServiceHosts.Clients"));

        var jobScheduler = new JobScheduler();
        await using (jobScheduler.ConfigureAwait(false))
        {
            builder.Services.AddSingleton(jobScheduler);

            var app = builder.Build();

            ServiceProviderFactory.Initialize(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            await jobScheduler.Initialize()
                              .ConfigureAwait(false);

            await jobScheduler.StartAsync()
                              .ConfigureAwait(false);

            using (var serviceProvider = ServiceProviderFactory.Create())
            {
                await singletons.Initialize(serviceProvider)
                                .ConfigureAwait(false);

                await app.RunAsync()
                         .ConfigureAwait(false);
            }
        }
    }
}