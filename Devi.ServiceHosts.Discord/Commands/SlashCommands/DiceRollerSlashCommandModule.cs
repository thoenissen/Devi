using System.Threading.Tasks;

using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord.Interactions;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands
{

    /// <summary>
    /// Dice Roller command
    /// </summary>
    [Group("diceroller", "Dice Roller creation")]
    public class DiceRollerSlashCommandModule : SlashCommandModuleBase
    {
        #region Properties
        /// <summary>
        /// Command handler
        /// </summary>
        public DiceRollerCommandHandler CommandHandler { get; set; }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Dice Roller
        /// </summary>
        /// <param name="diceCount"></param>
        /// <param name="diceType"></param>
        /// <returns></returns>
        [SlashCommand("roll", "Rolling random numbers of a specified die")]
        public Task Roll([Summary("DiceCount", "The number of dice that will be rolled")] int diceCount,
                         [Summary("DiceType", "The type of die that will be rolled")] int diceType) => CommandHandler.Roll(Context, diceCount, diceType);


        /// <summary>
        /// Dice Roller with a modifier value
        /// </summary>
        /// <param name="diceCount"></param>
        /// <param name="diceType"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [SlashCommand("rollmod", "Rolling random numbers with a modifier")]
        public Task RollMod([Summary("DiceCount", "The number of dice that will be rolled")] int diceCount,
                 [Summary("DiceType", "The type of die that will be rolled")] int diceType,
                 [Summary("Modifier", "The modifier you want to apply to the roll")] int modifier) => CommandHandler.RollMod(Context, diceCount, diceType, modifier);

        #endregion //Methods
    }
}
