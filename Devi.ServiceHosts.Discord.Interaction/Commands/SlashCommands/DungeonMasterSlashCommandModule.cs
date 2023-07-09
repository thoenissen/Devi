using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Handlers;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.SlashCommands;

/// <summary>
/// Dungeon master commands
/// </summary>
[Injectable<DungeonMasterSlashCommandModule>(ServiceLifetime.Transient)]
[DefaultMemberPermissions(GuildPermission.SendMessages)]
[Group("dm", "Dungeon master commands")]
public class DungeonMasterSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public PenAndPaperCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Campaign creation
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create-campaign", "Campaign creation")]
    public Task CreateCampaign() => CommandHandler.CreateCampaign(Context);

    /// <summary>
    /// Session creation
    /// </summary>
    /// <param name="date">Date</param>
    /// <param name="time">Time</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create-session", "Session creation")]
    public Task CreateSession([Summary("Date", "Date [dd.mm.yyyy]")]string date,
                              [Summary("Time", "Time [hh:mm]")]string time) => CommandHandler.CreateSession(Context, date, time);

    #endregion // Methods
}