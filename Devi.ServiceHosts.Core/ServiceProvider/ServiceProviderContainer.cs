using System;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Core.ServiceProvider;

/// <summary>
/// Service provider container
/// </summary>
internal class ServiceProviderContainer : IServiceProviderContainer
{
    #region Fields

    /// <summary>
    /// Scope
    /// </summary>
    private readonly IServiceScope _scope;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="scope">Scope</param>
    public ServiceProviderContainer(IServiceScope scope)
    {
        _scope = scope;
    }

    #endregion // Constructor

    #region IServiceProvider

    /// <summary>Gets the service object of the specified type.</summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>A service object of type <paramref name="serviceType" />.
    /// -or-
    /// <see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</returns>
    public object GetService(Type serviceType) => _scope.ServiceProvider.GetService(serviceType);

    #endregion // IServiceProvider

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _scope.Dispose();
    }

    #endregion // IDisposable
}