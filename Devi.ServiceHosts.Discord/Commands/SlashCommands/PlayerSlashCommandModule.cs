using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Player commands
/// </summary>
[Injectable<PlayerSlashCommandModule>(ServiceLifetime.Transient)]
[DefaultMemberPermissions(GuildPermission.SendMessages)]
[Group("player", "Player commands")]
public class PlayerSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public PlayerCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Create character
    /// </summary>
    /// <param name="characterName">Name</param>
    /// <param name="characterClass">Class</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create-character", "Character creation")]
    public Task AddCharacter([Summary("Name")] string characterName,
                             [Summary("Class")] Class characterClass) => CommandHandler.AddCharacter(Context, characterName, characterClass);

    /// <summary>
    /// Remove character
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("remove-character", "Character removing")]
    public Task RemoveCharacter() => CommandHandler.RemoveCharacter(Context);

    #endregion // Methods
}