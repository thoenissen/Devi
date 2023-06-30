using Devi.EventQueue.Core;
using Devi.EventQueue.Implementation;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.EventQueue.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    #region Methods

    /// <summary>
    /// Add RabbitMQ as implementation of <see cref="EventQueuePublishingService"/>
    /// </summary>
    /// <param name="serviceCollection">Service collection</param>
    /// <param name="hostName">Host name</param>
    /// <param name="virtualHost">Virtual host</param>
    public static void AddRabbitMQPublisher(this IServiceCollection serviceCollection, string hostName, string virtualHost)
    {
        var service = new EventQueuePublishingService(new RabbitMQPublisher(hostName, virtualHost));

        serviceCollection.AddSingleton(service);
    }

    /// <summary>
    /// Add RabbitMQ as implementation of <see cref="EventQueueSubscriberService"/>
    /// </summary>
    /// <param name="serviceCollection">Service collection</param>
    /// <param name="hostName">Host name</param>
    /// <param name="virtualHost">Virtual host</param>
    public static void AddRabbitMQSubscriber(this IServiceCollection serviceCollection, string hostName, string virtualHost)
    {
        var service = new EventQueueSubscriberService(new RabbitMQSubscriber(hostName, virtualHost));

        serviceCollection.AddSingleton(service);
    }

    #endregion // Methods
}