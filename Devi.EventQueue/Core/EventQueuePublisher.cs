using System.Text.Json;

using Devi.EventQueue.Configurations;
using Devi.EventQueue.Interface;

namespace Devi.EventQueue.Core;

/// <summary>
/// Event publisher
/// </summary>
public abstract class EventQueuePublisher
{
    /// <summary>
    /// Set internal event queue implementation
    /// </summary>
    /// <param name="eventQueue">Event queue</param>
    internal abstract void SetEventQueue(IPublisherQueueImplementation eventQueue);
}

/// <summary>
/// Event publisher
/// </summary>
/// <typeparam name="TConfiguration">Configuration</typeparam>
/// <typeparam name="TData">Event data</typeparam>
public class EventQueuePublisher<TConfiguration, TData> : EventQueuePublisher
    where TConfiguration : EventQueueConfiguration<TData>, new()
    where TData : class
{
    #region Fields

    /// <summary>
    /// Configuration
    /// </summary>
    private readonly TConfiguration _configuration;

    /// <summary>
    /// Event queue implementation
    /// </summary>
    private IPublisherQueueImplementation? _eventQueue;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    protected EventQueuePublisher()
    {
        _configuration = new TConfiguration();
    }

    #endregion // Constructor

    #region Public methods

    /// <summary>
    /// Publish event
    /// </summary>
    /// <param name="data">Data</param>
    public void Publish(TData data)
    {
        if (_eventQueue == null)
        {
            throw new InvalidOperationException();
        }

        var binaryData = new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(data));

        _eventQueue.Publish(binaryData);
    }

    #endregion // Public methods

    #region Internal methods

    /// <summary>
    /// Set internal event queue implementation
    /// </summary>
    /// <param name="eventQueue">Event queue</param>
    internal sealed override void SetEventQueue(IPublisherQueueImplementation eventQueue)
    {
        _eventQueue = eventQueue;
        _eventQueue.Initialize(_configuration);
    }

    #endregion // Internal methods
}