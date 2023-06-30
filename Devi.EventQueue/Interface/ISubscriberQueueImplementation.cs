using Devi.EventQueue.Configurations;

namespace Devi.EventQueue.Interface;

/// <summary>
/// Subscriber queue implementation
/// </summary>
internal interface ISubscriberQueueImplementation
{
    /// <summary>
    /// On message received action
    /// </summary>
    Action<ReadOnlyMemory<byte>>? OnMessageReceived { set; }

    /// <summary>
    /// SetImplementation
    /// </summary>
    /// <param name="configuration">Configuration</param>
    void Initialize(EventQueueConfiguration configuration);
}