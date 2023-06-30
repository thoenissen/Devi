using System.Reflection;

using Devi.EventQueue.Interface;

namespace Devi.EventQueue.Core;

/// <summary>
/// Manage event queue subscribers
/// </summary>
public class EventQueueSubscriberService
{
    #region Fields

    /// <summary>
    /// Implementation
    /// </summary>
    private readonly ISubscriberImplementation _implementation;

    /// <summary>
    /// Subscribers
    /// </summary>
    private readonly List<EventQueueSubscriber> _subscribers = new();

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="implementation">Event queue implementation (e.g. RabbitMQ)</param>
    internal EventQueueSubscriberService(ISubscriberImplementation implementation)
    {
        _implementation = implementation;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// SetImplementation subscriber
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="assemblies">Assemblies</param>
    public void Initialize(IServiceProvider serviceProvider, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes().Where(obj => typeof(EventQueueSubscriber).IsAssignableFrom(obj)))
            {
                if (serviceProvider.GetService(type) is EventQueueSubscriber subscriber)
                {
                    subscriber.SetEventQueue(_implementation.CreateQueue());

                    _subscribers.Add(subscriber);
                }
            }
        }
    }

    #endregion // Methods
}