using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Dialog.Base;
using Devi.ServiceHosts.Discord.Services.Discord;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        var builder = WebApplication.CreateBuilder(args);

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

        builder.Services.AddSingleton(new LocalizationService());
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