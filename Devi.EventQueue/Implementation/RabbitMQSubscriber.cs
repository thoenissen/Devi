using Devi.EventQueue.Interface;

using RabbitMQ.Client;

namespace Devi.EventQueue.Implementation;

/// <summary>
/// RabbitMQ subscriber implementation
/// </summary>
internal sealed class RabbitMQSubscriber : ISubscriberImplementation, IDisposable
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
    internal RabbitMQSubscriber(string hostName, string virtualHost)
    {
        var factory = new ConnectionFactory
                      {
                          HostName = hostName,
                          VirtualHost = virtualHost
                      };

        _connection = factory.CreateConnection();
    }

    #endregion // Constructor

    #region ISubscriberImplementation

    /// <summary>
    /// Create queue
    /// </summary>
    /// <returns>Queue</returns>
    public ISubscriberQueueImplementation CreateQueue()
    {
        return new RabbitMQSubscriberQueue(_connection);
    }

    #endregion // ISubscriberImplementation

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