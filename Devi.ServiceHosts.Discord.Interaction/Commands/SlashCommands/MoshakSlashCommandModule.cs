using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Interaction.Commands.Base;
using Devi.ServiceHosts.Discord.Interaction.Enumerations.Moshak;
using Devi.ServiceHosts.Discord.Interaction.Services.Moshak;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Commands.SlashCommands;

/// <summary>
/// Commands related to the Dungeons and Dragons world moshak
/// </summary>
[Injectable<MoshakSlashCommandModule>(ServiceLifetime.Transient)]
[DefaultMemberPermissions(GuildPermission.SendMessages)]
[Group("moshak", "Commands related to the Dungeons & Dragons world moshak")]
public class MoshakSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public MoashkCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Calculate timespans
    /// </summary>
    /// <param name="timeSpan">Time span</param>
    /// <param name="dimension">Dimension</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("timespan", "Character creation")]
    public Task TimeSpans([Summary("TimeSpan", "Timespan (xd|h|m|s)")] string timeSpan,
                          [Summary("Dimension")] Dimensions dimension) => CommandHandler.CalculateTimeSpan(Context, timeSpan, dimension);

    #endregion // Methods
}