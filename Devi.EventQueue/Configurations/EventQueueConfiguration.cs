namespace Devi.EventQueue.Configurations;

/// <summary>
/// Event queue configuration
/// </summary>
public class EventQueueConfiguration
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queueName">Queue name</param>
    protected EventQueueConfiguration(string queueName)
    {
        QueueName = queueName;
    }

    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; }

    /// <summary>
    /// Persistent storage
    /// </summary>
    public bool Persistent { get; protected set; } = true;

    /// <summary>
    /// Prefetch count
    /// </summary>
    public ushort PrefetchCount { get; protected set; } = 1;

    /// <summary>
    /// Consumer count
    /// </summary>
    public int ConsumerCounter { get; set; } = 1;

    #endregion // Constructor
}

/// <summary>
/// Event queue configuration
/// </summary>
/// <typeparam name="TData">Data</typeparam>
public class EventQueueConfiguration<TData> : EventQueueConfiguration
    where TData : class
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queueName">Queue name</param>
    protected EventQueueConfiguration(string queueName)
        : base(queueName)
    {
    }

    #endregion // Constructor
}