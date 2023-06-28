using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.MessageComponents;

/// <summary>
/// Looking for group commands
/// </summary>
public class LookingForGroupComponentCommandModule : LocatedInteractionModuleBase
{
    #region Constants

    /// <summary>
    /// Group
    /// </summary>
    public const string Group = "lfg";

    /// <summary>
    /// Command create
    /// </summary>
    public const string CommandCreate = "create";

    /// <summary>
    /// Command join
    /// </summary>
    public const string CommandJoin = "join";

    /// <summary>
    /// Command leave
    /// </summary>
    public const string CommandLeave = "leave";

    /// <summary>
    /// Command configuration
    /// </summary>
    public const string CommandConfiguration = "configuration";

    /// <summary>
    /// Configure menu options
    /// </summary>
    public const string CommandConfigureMenu = "configureMenu";

    #endregion // Constants

    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public LookingForGroupCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Commands

    /// <summary>
    /// Joining an appointment
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction($"{Group};{CommandCreate};")]
    public async Task Create()
    {
        await CommandHandler.StartCreation(Context)
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Joining an appointment
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction($"{Group};{CommandJoin};")]
    public async Task Join()
    {
        await Context.DeferAsync()
                     .ConfigureAwait(false);

        await CommandHandler.Join(Context)
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Leaving an appointment
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction($"{Group};{CommandLeave};")]
    public async Task Leave()
    {
        await Context.DeferAsync()
                     .ConfigureAwait(false);

        await CommandHandler.Leave(Context)
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Leaving an appointment
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction($"{Group};{CommandConfiguration};")]
    public async Task Configure()
    {
        await CommandHandler.Configure(Context)
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Configure menu options
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="value">Value</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction($"{Group};{CommandConfigureMenu};*")]
    public async Task ConfigureMenuOption(ulong appointmentMessageId, string value)
    {
        await CommandHandler.ConfigureMenuOption(Context, appointmentMessageId, value)
                            .ConfigureAwait(false);
    }

    #endregion // Commands
}