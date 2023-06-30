namespace Devi.EventQueue.Interface;

/// <summary>
/// Subscriber implementation
/// </summary>
internal interface ISubscriberImplementation
{
    /// <summary>
    /// Create queue
    /// </summary>
    /// <returns>Queue</returns>
    ISubscriberQueueImplementation CreateQueue();
}