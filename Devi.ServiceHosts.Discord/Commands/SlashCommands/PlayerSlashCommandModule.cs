using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Player commands
/// </summary>
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("create-character", "Character creation")]
    public Task AddCharacter() => CommandHandler.AddCharacter(Context);

    /// <summary>
    /// Remove character
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("remove-character", "Character removing")]
    public Task RemoveCharacter() => CommandHandler.RemoveCharacter(Context);

    #endregion // Methods
}