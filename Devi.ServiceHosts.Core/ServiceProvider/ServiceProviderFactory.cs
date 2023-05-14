using System;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Core.ServiceProvider;

/// <summary>
/// Service provider factory
/// </summary>
public class ServiceProviderFactory
{
    #region Fields

    /// <summary>
    /// Service provider
    /// </summary>
    private static IServiceProvider _serviceProvider;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="serviceProvider">Global service provider</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion // Methods

    #region Methods

    /// <summary>
    /// Create a new service provider representing a scope
    /// </summary>
    /// <returns>Service provider</returns>
    public static IServiceProviderContainer Create() => new ServiceProviderContainer(_serviceProvider.CreateScope());

    #endregion // Methods
}