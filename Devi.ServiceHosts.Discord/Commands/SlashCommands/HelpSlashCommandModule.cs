using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// 
/// </summary>
[Group("help", "Helper creation")]
public class HelpSlashCommandModule : SlashCommandModuleBase
{
    #region Properties
    /// <summary>
    /// Command Handler
    /// </summary>
    public HelpCommandHandler CommandHandler { get; set; }

    #endregion //Properties
    #region Methods
    /// <summary>
    /// A method to show all available commands
    /// </summary>
    /// <returns></returns>
    [SlashCommand("commands", "Shows a list of available commands and what they do")]
    public Task HelpCommands() => CommandHandler.HelpCommands(Context);

    #endregion //Methods
}