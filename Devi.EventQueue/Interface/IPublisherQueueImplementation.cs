using Devi.EventQueue.Configurations;

namespace Devi.EventQueue.Interface;

/// <summary>
/// Queue publisher implementation
/// </summary>
internal interface IPublisherQueueImplementation
{
    /// <summary>
    /// SetImplementation
    /// </summary>
    /// <param name="configuration">Configuration</param>
    void Initialize(EventQueueConfiguration configuration);

    /// <summary>
    /// Publish
    /// </summary>
    /// <param name="data">Data</param>
    void Publish(ReadOnlyMemory<byte> data);
}