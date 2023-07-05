using System.Reflection;

using Devi.Core.DependencyInjection;
using Devi.EventQueue.Core;
using Devi.EventQueue.Extensions;

using Discord;
using Discord.Rest;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Worker
{
    /// <summary>
    /// Program
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// Wait for program exit
        /// </summary>
        private static readonly TaskCompletionSource<bool> _waitForExitTaskSource = new();

        #endregion // Fields

        #region Methods

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main()
        {
            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddRabbitMQSubscriber(Environment.GetEnvironmentVariable("DEVI_RABBITMQ_HOST_NAME")!,
                                                    Environment.GetEnvironmentVariable("DEVI_RABBITMQ_VIRTUAL_HOST")!);

            var singletons = serviceCollection.AddServices(Assembly.GetExecutingAssembly(),
                                                           Assembly.Load("Devi.Core"),
                                                           Assembly.Load("Devi.ServiceHosts.Core"));

            var discordClient = new DiscordRestClient();

            await discordClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DEVI_DISCORD_TOKEN"))
                               .ConfigureAwait(false);

            serviceCollection.AddSingleton(discordClient);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            await using (serviceProvider.ConfigureAwait(false))
            {
                await singletons.Initialize(serviceProvider)
                                .ConfigureAwait(false);

                var subscriberService = serviceProvider.GetRequiredService<EventQueueSubscriberService>();

                subscriberService.Initialize(serviceProvider, Assembly.GetExecutingAssembly());

                await _waitForExitTaskSource.Task
                                            .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// The cancel key was pressed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = false;

            _waitForExitTaskSource.SetResult(true);
        }

        /// <summary>
        /// Occurs when the default application domain's parent process exits.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Argument</param>
        private static void OnProcessExit(object? sender, EventArgs e) => _waitForExitTaskSource.SetResult(true);

        #endregion // Methods
    }
}