using System;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;

using FluentScheduler;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Devi.ServiceHosts.WebApi.Jobs.Base;

/// <summary>
/// Asynchronous executing of a job
/// </summary>
public abstract class LocatedAsyncJob : IServiceScopeSupport, IAsyncJob, IDisposable
{
    #region Fields

    /// <summary>
    /// Localization group
    /// </summary>
    private LocalizationGroup _localizationGroup;

    /// <summary>
    /// Scope
    /// </summary>
    private IServiceProviderContainer _scope;

    #endregion // Fields

    #region Finalizer

    /// <summary>
    /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~LocatedAsyncJob()
    {
        Dispose(false);
    }

    #endregion // Finalizer

    #region Properties

    /// <summary>
    /// Localized group
    /// </summary>
    public LocalizationGroup LocalizationGroup => _localizationGroup ??= _scope?.GetRequiredService<LocalizationService>().GetGroup(GetType().Name);

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Executes the job
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public abstract Task ExecuteOverrideAsync();

    /// <summary>
    /// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <returns>A service object of type <typeparamref name="T"/>.</returns>
    protected T GetService<T>()
    {
        _scope ??= ServiceProviderFactory.Create();

        return _scope.GetRequiredService<T>();
    }

    #endregion // Methods

    #region IJob

    /// <summary>
    /// Executes the job.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ExecuteAsync()
    {
        try
        {
            Log.Information("[Job:{Type}] {Message}", GetType().Name, "Job started");

            await ExecuteOverrideAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[Job:{Type}] {Message}", GetType().Name, "Job execution failed");
        }
    }

    /// <summary>
    /// Executes the job
    /// </summary>
    public void Execute()
    {
        try
        {
            Log.Information("[Job:{Type}] {Message}", GetType().Name, "Job started");

            Task.Run(ExecuteOverrideAsync).Wait();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[Job:{Type}] {Message}", GetType().Name, "Job execution failed");
        }
    }

    #endregion // IJob

    #region IServiceScopeSupport

    /// <summary>
    /// Set the current scope
    /// </summary>
    /// <param name="scope">scope</param>
    public void SetScope(IServiceProviderContainer scope)
    {
        _scope = scope;
    }

    #endregion // IServiceScopeSupport

    #region IDisposable

    /// <summary>
    /// Internal IDisposable implementation
    /// </summary>
    /// <param name="disposing">Called from <see cref="Dispose()"/></param>?
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _scope?.Dispose();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    #endregion // IDisposable
}