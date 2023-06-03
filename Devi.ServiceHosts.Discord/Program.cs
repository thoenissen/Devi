using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Dialog.Base;
using Devi.ServiceHosts.Discord.Services.Discord;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenSearch.Net;

using Serilog;
using Serilog.Sinks.OpenSearch;

namespace Devi.ServiceHosts.Discord;

/// <summary>
/// Program
/// </summary>
internal class Program
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

        var singletons = new List<LocatedSingletonServiceBase>();

        foreach (var type in Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(obj => typeof(LocatedSingletonServiceBase).IsAssignableFrom(obj)
                                                && obj.IsAbstract == false))
        {
            var instance = (LocatedSingletonServiceBase)Activator.CreateInstance(type);
            if (instance != null)
            {
                builder.Services.AddSingleton(type, instance);

                singletons.Add(instance);
            }
        }

        foreach (var type in Assembly.Load("Devi.ServiceHosts.Core")
                                     .GetTypes()
                                     .Where(obj => typeof(LocatedSingletonServiceBase).IsAssignableFrom(obj)
                                                && obj.IsAbstract == false))
        {
            var instance = (LocatedSingletonServiceBase)Activator.CreateInstance(type);
            if (instance != null)
            {
                builder.Services.AddSingleton(type, instance);

                singletons.Add(instance);
            }
        }

        foreach (var type in Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(obj => (typeof(LocatedServiceBase).IsAssignableFrom(obj)
                                                 || typeof(DialogElementBase).IsAssignableFrom(obj))
                                                && typeof(InteractionContextContainer) != obj
                                                && obj.IsAbstract == false))
        {
            builder.Services.AddTransient(type);
        }

        builder.Services.AddSingleton<WebApiConnector>();

        var app = builder.Build();

        ServiceProviderFactory.Initialize(app.Services);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        using (var serviceProvider = ServiceProviderFactory.Create())
        {
            foreach (var singleton in singletons)
            {
                await singleton.Initialize(serviceProvider)
                               .ConfigureAwait(false);
            }

            await app.RunAsync()
                     .ConfigureAwait(false);
        }
    }
}