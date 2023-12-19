using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Worker.Data.Entity.Collections.Docker;
using Devi.ServiceHosts.Discord.Worker.Services;

using Discord.Rest;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

namespace Devi.ServiceHosts.Discord.Worker.Docker
{
    /// <summary>
    /// Forwarding Docker logs
    /// </summary>
    [Injectable<DockerLogForwardManager>(ServiceLifetime.Singleton)]
    internal sealed class DockerLogForwardManager : ISingletonInitialization, IAsyncDisposable
    {
        #region Fields

        /// <summary>
        /// Fields
        /// </summary>
        private IServiceProviderContainer? _serviceProvider;

        /// <summary>
        /// Forwarder
        /// </summary>
        private List<DockerLogForwarder> _forwarder = new();

        #endregion // Fields

        #region ISingletonInitialization

        /// <summary>
        /// Initialize
        /// </summary>
        /// <remarks>When this method is called all services are registered and can be resolved. But not all singleton services may be initialized.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Initialize()
        {
            _serviceProvider = ServiceProviderFactory.Create();

            var dockerFactory = _serviceProvider.GetRequiredService<DockerClientFactory>();
            var discordClient = _serviceProvider.GetRequiredService<DiscordRestClient>();

            var mongoFactory = _serviceProvider.GetRequiredService<MongoClientFactory>();

            foreach (var configuration in await mongoFactory.Create()
                                                            .GetDatabase(mongoFactory.Database)
                                                            .GetCollection<DockerForwardEntity>("DockerForwards")
                                                            .Find(_ => true)
                                                            .ToListAsync()
                                                            .ConfigureAwait(false))
            {
                _forwarder.Add(new DockerLogForwarder(dockerFactory, discordClient, configuration.ChannelId, configuration.ContainerName!));
            }

            foreach (var obj in _forwarder)
            {
                await obj.Start().ConfigureAwait(false);
            }
        }

        #endregion // ISingletonInitialization

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            foreach (var obj in _forwarder)
            {
                await obj.Stop().ConfigureAwait(false);
            }

            _serviceProvider?.Dispose();
        }

        #endregion // IDisposable
    }
}