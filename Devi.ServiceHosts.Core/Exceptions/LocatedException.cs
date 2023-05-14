using System;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Core.Exceptions;

/// <summary>
/// Exception with located messages
/// </summary>
public abstract class LocatedException : Exception, IDisposable
{
    #region Fields

    /// <summary>
    /// Service provider
    /// </summary>
    private IServiceProviderContainer _serviceProvider;

    /// <summary>
    /// Localization group
    /// </summary>
    private LocalizationGroup _localizationGroup;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Localization group
    /// </summary>
    protected LocalizationGroup LocalizationGroup
    {
        get
        {
            if (_localizationGroup == null)
            {
                _serviceProvider = ServiceProviderFactory.Create();

                _localizationGroup = _serviceProvider.GetRequiredService<LocalizationService>().GetGroup(GetType().Name);
            }

            return _localizationGroup;
        }
    }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns localized message
    /// </summary>
    /// <returns>Message</returns>
    public abstract string GetLocalizedMessage();

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Called from <see cref="Dispose()"/>?</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceProvider?.Dispose();
        }
    }

    #endregion // IDisposable
}