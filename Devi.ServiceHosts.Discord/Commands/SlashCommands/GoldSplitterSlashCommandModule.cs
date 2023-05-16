using System.Threading.Tasks;
using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;


namespace Devi.ServiceHosts.Discord.Commands.SlashCommands
{
    /// <summary>
    /// Gold Splitter command
    /// </summary>
    [Group("goldsplitter", "Gold Splitter creation")]
    public class GoldSplitterSlashCommandModule : SlashCommandModuleBase
    {
        #region Properties
        /// <summary>
        /// Command handler
        /// </summary>
        public GoldSplitterCommandHandler CommandHandler { get; set; }

        #endregion //Properties

        #region Methods
        /// <summary>
        /// A method to split earned Currency between party members
        /// </summary>
        /// <param name="goldCount"></param>
        /// <param name="silverCount"></param>
        /// <param name="copperCount"></param>
        /// <param name="playerCount"></param>
        /// <returns></returns>
        [SlashCommand("split", "Splitting Gold, Silver and Copper between a specified number of individuals")]
        public Task Split([Summary("Gold", "The number of gold pieces you want to split")] int goldCount,
                          [Summary("Silver", "The number of silver pieces you want to split")] int silverCount,
                          [Summary("Copper", "The number of copper pieces you want to split")] int copperCount,
                          [Summary("Players", "The number of individuals the currency will be split to")] int playerCount) => CommandHandler.Split(Context, goldCount, silverCount, copperCount, playerCount);

        #endregion //Methods
    }
}
