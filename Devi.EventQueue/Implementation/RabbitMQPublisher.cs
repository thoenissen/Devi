using Devi.EventQueue.Interface;

using RabbitMQ.Client;

namespace Devi.EventQueue.Implementation;

/// <summary>
/// RabbitMQ publisher implementation
/// </summary>
internal sealed class RabbitMQPublisher : IPublisherImplementation, IDisposable
{
    #region Fields

    /// <summary>
    /// Connection
    /// </summary>
    private readonly IConnection _connection;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hostName">Host name</param>
    /// <param name="virtualHost">Virtual host</param>
    internal RabbitMQPublisher(string hostName, string virtualHost)
    {
        var factory = new ConnectionFactory
                      {
                          HostName = hostName,
                          VirtualHost = virtualHost
                      };

        _connection = factory.CreateConnection();
    }

    #endregion // Constructor

    #region IPublisherImplementation

    /// <summary>
    /// Create queue
    /// </summary>
    /// <returns>Queue</returns>
    IPublisherQueueImplementation IPublisherImplementation.CreateQueue()
    {
        return new RabbitMQPublisherQueue(_connection);
    }

    #endregion // IPublisherImplementation

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
    }

    #endregion // IDisposable
}