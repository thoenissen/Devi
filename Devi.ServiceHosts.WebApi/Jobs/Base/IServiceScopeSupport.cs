using Devi.ServiceHosts.Core.ServiceProvider;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.WebApi.Jobs.Base;

/// <summary>
/// <see cref="IServiceScope"/> support
/// </summary>
public interface IServiceScopeSupport
{
    /// <summary>
    /// Set the current scope
    /// </summary>
    /// <param name="scope">scope</param>
    void SetScope(IServiceProviderContainer scope);
}