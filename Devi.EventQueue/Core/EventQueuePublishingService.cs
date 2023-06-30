using System.Collections.Concurrent;

using Devi.EventQueue.Interface;

namespace Devi.EventQueue.Core;

/// <summary>
/// Publishing events
/// </summary>
public class EventQueuePublishingService
{
    #region Fields

    /// <summary>
    /// Publisher
    /// </summary>
    private readonly ConcurrentDictionary<Type, EventQueuePublisher> _publisher = new();

    /// <summary>
    /// Implementation
    /// </summary>
    private readonly IPublisherImplementation _implementation;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="implementation">Event queue implementation (e.g. RabbitMQ)</param>
    internal EventQueuePublishingService(IPublisherImplementation implementation)
    {
        _implementation = implementation;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Get event queue publisher
    /// </summary>
    /// <typeparam name="T">Publisher type</typeparam>
    /// <returns>Publisher</returns>
    public T GetPublisher<T>() where T : EventQueuePublisher, new()
    {
        return (T)_publisher.GetOrAdd(typeof(T),
                                      type =>
                                      {
                                          var publisher = new T();

                                          publisher.SetEventQueue(_implementation.CreateQueue());

                                          return publisher;
                                      });
    }

    #endregion // Methods
}