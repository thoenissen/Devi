using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Core.Exceptions;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;

namespace Devi.ServiceHosts.Discord.Interaction.Services.Discord;

/// <summary>
/// Discord client
/// </summary>
[Injectable<DiscordClient>(ServiceLifetime.Singleton)]
public sealed class DiscordClient : LocatedSingletonServiceBase,
                                    ISingletonInitialization,
                                    IDisposable
{
    #region Fields

    /// <summary>
    /// Service provider
    /// </summary>
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// Debug channel id
    /// </summary>
    private ulong _debugChannel;

    /// <summary>
    /// Last disconnect exception
    /// </summary>
    private Exception _lastDisconnect;

    /// <summary>
    /// Interactivity service
    /// </summary>
    private InteractivityService _interactivityService;

    /// <summary>
    /// Localization service
    /// </summary>
    private LocalizationService _localizationService;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="interactivityService">Interactivity service</param>
    public DiscordClient(IServiceProvider serviceProvider, LocalizationService localizationService, InteractivityService interactivityService)
        : base(localizationService)
    {
        _serviceProvider = serviceProvider;
        _localizationService = localizationService;
        _interactivityService = interactivityService;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Interaction service
    /// </summary>
    internal InteractionService Interaction { get; private set; }

    /// <summary>
    /// Client
    /// </summary>
    internal DiscordSocketClient Client { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Start the discord bot
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task StartAsync()
    {
        var debugChannel = Environment.GetEnvironmentVariable("DEVI_DEBUG_CHANNEL");
        if (string.IsNullOrWhiteSpace(debugChannel) == false)
        {
            _debugChannel = Convert.ToUInt64(debugChannel);
        }

        var config = new DiscordSocketConfig
                     {
                         LogLevel = LogSeverity.Info,
                         MessageCacheSize = 100,
                         GatewayIntents = GatewayIntents.Guilds
                                        | GatewayIntents.GuildMembers
                                        | GatewayIntents.GuildEmojis
                                        | GatewayIntents.GuildIntegrations
                                        | GatewayIntents.GuildVoiceStates
                                        | GatewayIntents.GuildPresences
                                        | GatewayIntents.GuildMessages
                                        | GatewayIntents.GuildMessageReactions
                                        | GatewayIntents.DirectMessages
                                        | GatewayIntents.DirectMessageReactions
                                        | GatewayIntents.MessageContent
                                        | GatewayIntents.GuildScheduledEvents
                     };

        Client = new DiscordSocketClient(config);
        Client.InteractionCreated += OnInteractionCreated;
        Client.Log += OnDiscordClientLog;

        var interactionConfiguration = new InteractionServiceConfig
                                       {
                                           LogLevel = LogSeverity.Info,
                                           DefaultRunMode = RunMode.Async,
                                           ThrowOnError = true
                                       };

        Interaction = new InteractionService(Client, interactionConfiguration);
        Interaction.Log += OnInteractionServiceLog;
        Interaction.ComponentCommandExecuted += OnComponentCommandExecuted;
        Interaction.SlashCommandExecuted += OnSlashCommandExecuted;
        Interaction.ModalCommandExecuted += OnModalCommandExecuted;

        Client.Connected += OnConnected;
        Client.Disconnected += OnDisconnected;

        await Interaction.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider)
                         .ConfigureAwait(false);

        _interactivityService.SetDiscordClient(Client);

        await Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DEVI_DISCORD_TOKEN"))
                            .ConfigureAwait(false);

        await Client.StartAsync()
                    .ConfigureAwait(false);
    }

    /// <summary>
    /// The discord client connected
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    private async Task OnConnected()
    {
        if (_debugChannel > 0)
        {
            var channel = await Client.GetChannelAsync(_debugChannel)
                                              .ConfigureAwait(false);

            if (channel is ITextChannel textChannel)
            {
                var message = new StringBuilder();

                message.AppendLine("The connection to Discord has been established.");

                message.AppendLine("```");

                message.AppendLine($"Version: {new FileInfo(Assembly.GetExecutingAssembly().Location).CreationTime:yyyy-MM-dd HH:mm:ss}");

                if (_lastDisconnect != null)
                {
                    message.Append("Last disconnect message: ");
                    message.AppendLine(_lastDisconnect.ToString());

                    _lastDisconnect = null;
                }

                message.AppendLine("```");

                await textChannel.SendMessageAsync(message.ToString())
                                 .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// The discord client disconnected
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    private Task OnDisconnected(Exception ex)
    {
        _lastDisconnect = ex;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Discord client logging
    /// </summary>
    /// <param name="e">Argument</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task OnDiscordClientLog(LogMessage e) => OnLogMessage("Client", e);

    /// <summary>
    /// Interaction service logging
    /// </summary>
    /// <param name="e">Argument</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task OnInteractionServiceLog(LogMessage e) => OnLogMessage("Interaction", e);

    /// <summary>
    /// Logging
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="message">Message</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task OnLogMessage(string type, LogMessage message)
    {
        Log.Write(message.Severity switch
                  {
                      LogSeverity.Critical => LogEventLevel.Fatal,
                      LogSeverity.Error => LogEventLevel.Error,
                      LogSeverity.Warning => LogEventLevel.Warning,
                      LogSeverity.Info => LogEventLevel.Information,
                      LogSeverity.Verbose => LogEventLevel.Verbose,
                      LogSeverity.Debug => LogEventLevel.Debug,
                      _ => throw new ArgumentOutOfRangeException()
                  },
                  message.Exception,
                  "[Discord:{Type}:{Source}] {Message}",
                  type,
                  message.Source,
                  message.Message);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Interaction created
    /// </summary>
    /// <param name="e">Arguments</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnInteractionCreated(SocketInteraction e)
    {
        var context = new InteractionContextContainer(Client, e, _interactivityService, _localizationService);

        try
        {
            await Interaction.ExecuteCommandAsync(context, context.ServiceProvider)
                              .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleInteractionException(context, ex).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handling command exceptions
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="ex">Exception</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task HandleInteractionException(InteractionContextContainer context, Exception ex)
    {
        if (ex is LocatedException)
        {
            if (ex is UserMessageException userException)
            {
                await context.SendMessageAsync($"{context.User.Mention} {userException.GetLocalizedMessage()}", ephemeral: true)
                             .ConfigureAwait(false);
            }
        }
        else
        {
            if (((IInteractionContext)context).Interaction?.Data is IComponentInteractionData interactionData)
            {
                Log.Error(ex, "[Discord:{Type}:{CustomId}:{UserId}] {Message}", "Unknown", interactionData.CustomId, context.User.ToString(), "Unhandled execution error");
            }
            else
            {
                Log.Error(ex, "[Discord:{Type}:{UserId}] {Message}", "Unknown", context.User.ToString(), "Unhandled execution error");
            }

            await context.SendMessageAsync(LocalizationGroup.GetFormattedText("CommandFailedMessage", "The command could not be executed."),
                                           ephemeral: true)
                         .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Component command executed
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="context">Context</param>
    /// <param name="result">Result</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnComponentCommandExecuted(ComponentCommandInfo command, IInteractionContext context, IResult result)
    {
        if (context is InteractionContextContainer container)
        {
            if (((IInteractionContext)container).Interaction?.Data is IComponentInteractionData interactionData)
            {
                Log.Error("[Discord:{Type}:{CustomId}:{UserId}] {Message}", "Component", interactionData.CustomId, context.User.ToString(), "Component executed");
            }

            using (container)
            {
                if (result.IsSuccess == false)
                {
                    switch (result.Error)
                    {
                        case InteractionCommandError.Exception:
                            {
                                if (result is ExecuteResult executeResult)
                                {
                                    await HandleInteractionException(container, executeResult.Exception).ConfigureAwait(false);
                                }
                            }
                            break;
                        case InteractionCommandError.UnknownCommand:
                        case InteractionCommandError.ParseFailed:
                        case InteractionCommandError.ConvertFailed:
                        case InteractionCommandError.BadArgs:
                        case InteractionCommandError.UnmetPrecondition:
                        case InteractionCommandError.Unsuccessful:
                        case null:
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Modal command executed
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="context">Context</param>
    /// <param name="result">Result</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnModalCommandExecuted(ModalCommandInfo command, IInteractionContext context, IResult result)
    {
        if (context is InteractionContextContainer container)
        {
            if (((IInteractionContext)container).Interaction?.Data is IComponentInteractionData interactionData)
            {
                Log.Error("[Discord:{Type}:{CustomId}:{UserId}] {Message}", "Modal", interactionData.CustomId, context.User.ToString(), "Modal executed");
            }

            using (container)
            {
                if (result.IsSuccess == false)
                {
                    switch (result.Error)
                    {
                        case InteractionCommandError.Exception:
                            {
                                if (result is ExecuteResult executeResult)
                                {
                                    await HandleInteractionException(container, executeResult.Exception).ConfigureAwait(false);
                                }
                            }
                            break;
                        case InteractionCommandError.UnknownCommand:
                        case InteractionCommandError.ParseFailed:
                        case InteractionCommandError.ConvertFailed:
                        case InteractionCommandError.BadArgs:
                        case InteractionCommandError.UnmetPrecondition:
                        case InteractionCommandError.Unsuccessful:
                        case null:
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Slash command executed
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="context">Context</param>
    /// <param name="result">Result</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
    {
        if (context is InteractionContextContainer container)
        {
            if (((IInteractionContext)container).Interaction?.Data is IComponentInteractionData interactionData)
            {
                Log.Error("[Discord:{Type}:{CustomId}:{UserId}] {Message}", "Component", interactionData.CustomId, context.User.ToString(), "SlashCommand executed");
            }

            using (container)
            {
                if (result.IsSuccess == false)
                {
                    switch (result.Error)
                    {
                        case InteractionCommandError.Exception:
                            {
                                if (result is ExecuteResult executeResult)
                                {
                                    await HandleInteractionException(container, executeResult.Exception).ConfigureAwait(false);
                                }
                            }
                            break;
                        case InteractionCommandError.UnknownCommand:
                        case InteractionCommandError.ParseFailed:
                        case InteractionCommandError.ConvertFailed:
                        case InteractionCommandError.BadArgs:
                        case InteractionCommandError.UnmetPrecondition:
                        case InteractionCommandError.Unsuccessful:
                        case null:
                        default:
                            break;
                    }
                }
            }
        }
    }

    #endregion // Methods

    #region IAsyncDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interaction != null)
        {
            Interaction.Dispose();
            Interaction = null;
        }

        if (Client != null)
        {
            Client.Connected -= OnConnected;
            Client.Disconnected -= OnDisconnected;

            await Client.LogoutAsync()
                                .ConfigureAwait(false);
            await Client.DisposeAsync()
                                .ConfigureAwait(false);
            Client = null;
        }
    }

    #endregion // IAsyncDisposable

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (Interaction != null)
        {
            Interaction.Dispose();
            Interaction = null;
        }

        if (Client != null)
        {
            Client.Connected -= OnConnected;
            Client.Disconnected -= OnDisconnected;
            Client.Dispose();
            Client = null;
        }
    }

    #endregion // IDisposable

    #region ISingletonInitialization

    /// <summary>
    /// Initialize
    /// </summary>
    /// <remarks>When this method is called all services are registered and can be resolved.  But not all singleton services may be initialized. </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Initialize()
    {
        await StartAsync().ConfigureAwait(false);
    }

    #endregion // ISingletonInitialization
}