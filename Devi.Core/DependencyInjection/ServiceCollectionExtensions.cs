using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.Core.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    #region Methods

    /// <summary>
    /// Add all services marked with the Attribute <see cref="InjectableAttribute"/>
    /// </summary>
    /// <param name="serviceCollection">Service collection</param>
    /// <param name="assemblies">Assemblies</param>
    /// <returns>Singleton container</returns>
    public static SingletonInitializationContainer AddServices(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        var container = new SingletonInitializationContainer();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var attribute in type.GetCustomAttributes<InjectableAttribute>())
                {
                    attribute.Register(serviceCollection, type);

                    if (type.IsAssignableTo(typeof(ISingletonInitialization)))
                    {
                        container.Add(type);
                    }
                }
            }
        }

        return container;
    }

    #endregion // Methods
}