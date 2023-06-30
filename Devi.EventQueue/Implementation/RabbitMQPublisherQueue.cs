using Devi.EventQueue.Configurations;
using Devi.EventQueue.Interface;

using RabbitMQ.Client;

namespace Devi.EventQueue.Implementation;

/// <summary>
/// RabbitMQ queue implementation
/// </summary>
internal class RabbitMQPublisherQueue : IPublisherQueueImplementation
{
    #region Fields

    /// <summary>
    /// Connection
    /// </summary>
    private readonly IConnection _connection;

    /// <summary>
    /// Configuration
    /// </summary>
    private EventQueueConfiguration? _configuration;

    /// <summary>
    /// Model
    /// </summary>
    private IModel? _model;

    /// <summary>
    /// Properties
    /// </summary>
    private IBasicProperties? _properties;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connection">Connection</param>
    internal RabbitMQPublisherQueue(IConnection connection)
    {
        _connection = connection;
    }

    #endregion // Constructor

    #region IPublisherQueueImplementation

    /// <summary>
    /// SetImplementation
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public void Initialize(EventQueueConfiguration configuration)
    {
        _configuration = configuration;
        _model = _connection.CreateModel();
        _properties = _model.CreateBasicProperties();
        _properties.Persistent = _configuration.Persistent;
    }

    /// <summary>
    /// Publish
    /// </summary>
    /// <param name="data">Data</param>
    public void Publish(ReadOnlyMemory<byte> data)
    {
        _model.BasicPublish(string.Empty,
                            _configuration?.QueueName ?? throw new InvalidOperationException(),
                            _properties,
                            data);
    }

    #endregion // IPublisherQueueImplementation
}