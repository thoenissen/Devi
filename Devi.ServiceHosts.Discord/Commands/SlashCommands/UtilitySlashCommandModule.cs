using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Utility commands
/// </summary>
public class UtilitySlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public UtilityCommandHandler CommandHandler { get; set; }

    #endregion //Properties

    #region Methods

    /// <summary>
    /// Splitting gold, silver and copper between players
    /// </summary>
    /// <param name="goldCount">Gold count</param>
    /// <param name="silverCount">Silver count</param>
    /// <param name="copperCount">Copper count</param>
    /// <param name="playerCount">Player count</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("split-coins", "Splitting Gold, Silver and Copper between a specified number of players")]
    public Task Split([Summary("Gold", "The number of gold pieces you want to split")] int goldCount,
                      [Summary("Silver", "The number of silver pieces you want to split")] int silverCount,
                      [Summary("Copper", "The number of copper pieces you want to split")] int copperCount,
                      [Summary("Players", "The number of players the currency will be split to")] int playerCount) => CommandHandler.SplitCoins(Context, goldCount, silverCount, copperCount, playerCount);

    #endregion //Methods
}