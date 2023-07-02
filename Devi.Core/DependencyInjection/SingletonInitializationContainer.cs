namespace Devi.Core.DependencyInjection;

/// <summary>
/// Container to collect all singletons to be initialized
/// </summary>
public class SingletonInitializationContainer
{
    #region Fields

    /// <summary>
    /// Types
    /// </summary>
    private readonly List<Type> _types = new();

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Add type
    /// </summary>
    /// <param name="type">Type</param>
    internal void Add(Type type)
    {
        _types.Add(type);
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Initialize(IServiceProvider serviceProvider)
    {
        foreach (var type in _types)
        {
            if (serviceProvider.GetService(type) is ISingletonInitialization service)
            {
                await service.Initialize()
                             .ConfigureAwait(false);
            }
        }
    }

    #endregion // Methods
}