namespace Devi.Core.DependencyInjection;

/// <summary>
/// Initialization of a singleton service
/// </summary>
public interface ISingletonInitialization
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <remarks>When this method is called all services are registered and can be resolved. But not all singleton services may be initialized.</remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Initialize();
}