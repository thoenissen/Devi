using Microsoft.Extensions.DependencyInjection;

namespace Devi.Core.DependencyInjection;

/// <summary>
/// Marks a class as injectable
/// </summary>
public abstract class InjectableAttribute : Attribute
{
    #region Methods

    /// <summary>
    /// Register service
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="type">Implementation type</param>
    internal abstract void Register(IServiceCollection services, Type type);

    #endregion // Methods
}

/// <summary>
/// Marks a class as injectable
/// </summary>
/// <typeparam name="T">Type the service is providing</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class InjectableAttribute<T> : InjectableAttribute where T : class
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceLifetime">Service life time</param>
    public InjectableAttribute(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Service life time
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; }

    #endregion // Properties

    #region InjectableAttribute

    /// <summary>
    /// Register service
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="type">Implementation type</param>
    internal override void Register(IServiceCollection services, Type type)
    {
        services.Add(new ServiceDescriptor(typeof(T), type, ServiceLifetime));
    }

    #endregion // InjectableAttribute
}