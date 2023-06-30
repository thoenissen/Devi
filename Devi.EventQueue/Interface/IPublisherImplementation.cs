namespace Devi.EventQueue.Interface;

/// <summary>
/// Event publisher implementation
/// </summary>
internal interface IPublisherImplementation
{
    /// <summary>
    /// Create queue
    /// </summary>
    /// <returns>Queue</returns>
    IPublisherQueueImplementation CreateQueue();
}