using Devi.EventQueue.Configurations;
using Devi.EventQueue.Interface;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Devi.EventQueue.Implementation;

/// <summary>
/// RabbitMQ subscriber implementation
/// </summary>
internal class RabbitMQSubscriberQueue : ISubscriberQueueImplementation
{
    #region Fields

    /// <summary>
    /// Consumers
    /// </summary>
    private readonly List<EventingBasicConsumer> _consumers = new();

    /// <summary>
    /// Connection
    /// </summary>
    private readonly IConnection _connection;

    /// <summary>
    /// Model
    /// </summary>
    private IModel? _model;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connection">Connection</param>
    public RabbitMQSubscriberQueue(IConnection connection)
    {
        _connection = connection;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Received consumer message
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    private void OnConsumerReceived(object? sender, BasicDeliverEventArgs e)
    {
        OnMessageReceived?.Invoke(e.Body)
                         .ContinueWith(t => _model?.BasicAck(e.DeliveryTag, false));
    }

    #endregion // Methods

    #region ISubscriberQueueImplementation

    /// <summary>
    /// On message received action
    /// </summary>
    public Func<ReadOnlyMemory<byte>, Task>? OnMessageReceived { private get; set; }

    /// <summary>
    /// SetImplementation
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public void Initialize(EventQueueConfiguration configuration)
    {
        _model = _connection.CreateModel();

        _model.QueueDeclare(configuration.QueueName,
                            configuration.Persistent,
                            false,
                            false,
                            null);

        _model.BasicQos(0, configuration.PrefetchCount, false);

        for (var count = 0; count < configuration.ConsumerCounter; count++)
        {
            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += OnConsumerReceived;

            _model.BasicConsume(configuration.QueueName, false, consumer);

            _consumers.Add(consumer);
        }
    }

    #endregion // ISubscriberQueueImplementation
}