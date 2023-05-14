using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.WebApi.Data.Entity;
using Devi.ServiceHosts.WebApi.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<DiscordConnector>();

        builder.Services.AddTransient<RepositoryFactory>();

        var singletons = new List<LocatedSingletonServiceBase>();

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

            await jobScheduler.Initialize(app.Services)
                              .ConfigureAwait(false);

            await jobScheduler.StartAsync()
                              .ConfigureAwait(false);

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
}