using Devi.ServiceHosts.Discord.Worker.Services;

using Discord;
using Discord.Rest;

using Docker.DotNet;
using Docker.DotNet.Models;

using Serilog;

namespace Devi.ServiceHosts.Discord.Worker.Docker
{
    /// <summary>
    /// Forwarding Docker logs
    /// </summary>
    internal class DockerLogForwarder : IProgress<string>
    {
        #region Fields

        /// <summary>
        /// Docker factory
        /// </summary>
        private readonly DockerClientFactory _dockerFactory;

        /// <summary>
        /// Discord client
        /// </summary>
        private readonly DiscordRestClient _discordClient;

        /// <summary>
        /// Channel ID
        /// </summary>
        private readonly ulong _channelId;

        /// <summary>
        /// Container name
        /// </summary>
        private readonly string _containerName;

        /// <summary>
        /// Docker client
        /// </summary>
        private DockerClient? _dockerClient;

        /// <summary>
        /// Cancellation token source
        /// </summary>
        private CancellationTokenSource? _tokenSource;

        /// <summary>
        /// Forward task
        /// </summary>
        private Task? _task;

        /// <summary>
        /// Target channel
        /// </summary>
        private ITextChannel? _textChannel;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dockerFactory">Docker factory</param>
        /// <param name="discordClient">Discord client</param>
        /// <param name="channelId">Channel ID</param>
        /// <param name="containerName">Container name</param>
        public DockerLogForwarder(DockerClientFactory dockerFactory,
                                  DiscordRestClient discordClient,
                                  ulong channelId,
                                  string containerName)
        {
            _dockerFactory = dockerFactory;
            _discordClient = discordClient;
            _channelId = channelId;
            _containerName = containerName;
        }

        #endregion // Constructor

        #region Methods

        /// <summary>
        /// Start
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Start()
        {
            try
            {
                _dockerClient = _dockerFactory.Create();

                var containers = await _dockerClient.Containers
                                                    .ListContainersAsync(new ContainersListParameters
                                                                         {
                                                                             Filters = new Dictionary<string, IDictionary<string, bool>>
                                                                                       {
                                                                                           ["name"] = new Dictionary<string, bool>
                                                                                                      {
                                                                                                          [_containerName] = true
                                                                                                      }
                                                                                       }
                                                                         })
                                                    .ConfigureAwait(false);

                var id = containers.FirstOrDefault()?.ID;

                if (string.IsNullOrWhiteSpace(id) == false)
                {
                    _tokenSource = new CancellationTokenSource();

                    var parameters = new ContainerLogsParameters
                                     {
                                         ShowStderr = true,
                                         ShowStdout = true,
                                         Follow = true,
                                         Since = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                                     };

                    _task = _dockerClient.Containers
                                         .GetContainerLogsAsync(id, parameters, CancellationToken.None, this);
                }
                else
                {
                    Log.Error("Could not find docker container {ContainerName} for log forwarding.", _containerName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown error while starting docker log forwarding.");
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Stop()
        {
            try
            {
                _tokenSource?.Cancel();

                if (_task != null)
                {
                    await _task.ConfigureAwait(false);
                }

                _tokenSource?.Dispose();
                _dockerClient?.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown error while stopping docker log forwarding.");
            }
        }

        #endregion // Methods

        #region IProgress<in string>

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        public void Report(string value)
        {
            if (value?.Length > 0)
            {
                try
                {
                    _textChannel ??= _discordClient.GetChannelAsync(_channelId)
                                                   .Result as ITextChannel;

                    // Remove tty header
                    if (value[0] < 3 && value.Length >= 8)
                    {
                        value = value[8..];
                    }

                    _textChannel?.SendMessageAsync(value).Wait();
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to forward message.");
                }
            }
        }

        #endregion // IProgress<in string>
    }
}