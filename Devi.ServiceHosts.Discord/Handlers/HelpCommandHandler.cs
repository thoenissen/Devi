using System;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

using Discord;

namespace Devi.ServiceHosts.Discord.Handlers
{
    /// <summary>
    /// Helper commands
    /// </summary>
    public class HelpCommandHandler : LocatedServiceBase
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizationService"></param>
        public HelpCommandHandler(LocalizationService localizationService)
            : base(localizationService) { }

        #endregion //Constructor

        #region Methods
        /// <summary>
        /// Shows a summary of commands that can be used and explains what they do
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task HelpCommands(InteractionContextContainer context)
        {
            var builder = new System.Text.StringBuilder();
            //Creating the title
            var title = "The actions that I can take each turn: " + Environment.NewLine;
            builder.AppendLine(title);

            //Creating a list of commands Devi can provide

            //Diceroller Commands
            var dicerollerexplain = Format.Bold("Diceroller: ")
                                    + Environment.NewLine + "These commands can be used to roll some dice."
                                    + Environment.NewLine + "- " + Format.Bold("/diceroller roll ") + Format.Code("dice-count: dice-type: ") + "  This command rolls random numbers on a specified type of die"
                                    + Environment.NewLine + "- " + Format.Bold("/diceroller rollmod ") + Format.Code("dice-count: dice-type: modifier: ") + " This command rolls random numbers on a die and adds a modifier"
                                    + Environment.NewLine;
            builder.AppendLine(dicerollerexplain);

            //Goldsplitter Commands
            var goldsplitterexplain = Format.Bold("Gold Splitter: ")
                                      + Environment.NewLine + "This command can be used to split currency between party members"
                                      + Environment.NewLine + "- " + Format.Bold("/goldsplitter split ") + Format.Code("gold: silver: copper: players: ") + " This command splits the three main currencies between a given number of players"
                                      + Environment.NewLine;
            builder.AppendLine(goldsplitterexplain);

            //Creating the footer message
            var footer = Environment.NewLine + "Always happy to be of service!";
            builder.AppendLine(footer);

            await context.ReplyAsync(builder.ToString()).ConfigureAwait(false);
        }

        #endregion //Methods
    }
}
