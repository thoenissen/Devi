using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.MessageComponents;

/// <summary>
/// Pen and paper component commands
/// </summary>
public class PenAndPaperMessageComponentCommandModule : LocatedInteractionModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public PenAndPaperCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Commands

    /// <summary>
    /// Join session
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("pnp;session;join")]
    public Task JoinSession() => CommandHandler.JoinSession(Context);

    /// <summary>
    /// Leave session
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("pnp;session;leave")]
    public Task DeleteSession() => CommandHandler.LeaveSession(Context);

    /// <summary>
    /// Campaign settings
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ComponentInteraction("pnp;campaign;settings")]
    public Task CampaignSettings() => CommandHandler.CampaignSettings(Context);

    #endregion // Commands
}