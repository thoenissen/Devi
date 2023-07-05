using System.Text.Json;

using Devi.EventQueue.Configurations;
using Devi.EventQueue.Interface;

namespace Devi.EventQueue.Core;

/// <summary>
/// Event queue subscriber
/// </summary>
public abstract class EventQueueSubscriber
{
    /// <summary>
    /// Set event queue
    /// </summary>
    /// <param name="queue">Queue</param>
    internal abstract void SetEventQueue(ISubscriberQueueImplementation queue);
}

/// <summary>
/// Event queue subscriber
/// </summary>
/// <typeparam name="TConfiguration">Configuration</typeparam>
/// <typeparam name="TData">Data</typeparam>
public abstract class EventQueueSubscriber<TConfiguration, TData> : EventQueueSubscriber
    where TConfiguration : EventQueueConfiguration<TData>, new()
    where TData : class
{
    #region Methods

    /// <summary>
    /// On message received
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnMessageReceived(ReadOnlyMemory<byte> data)
    {
        await Execute(JsonSerializer.Deserialize<TData>(data.Span)).ConfigureAwait(false);
    }

    #endregion // Methods

    #region EventQueueSubscriber

    /// <summary>
    /// Execute event
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task Execute(TData? data);

    /// <summary>
    /// Set event queue
    /// </summary>
    /// <param name="queue">Queue</param>
    internal sealed override void SetEventQueue(ISubscriberQueueImplementation queue)
    {
        queue.OnMessageReceived = OnMessageReceived;
        queue.Initialize(new TConfiguration());
    }

    #endregion // EventQueueSubscriber
}