using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;
using Discord;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Devi.ServiceHosts.Discord.Handlers
{
    /// <summary>
    /// DiceRoller commands
    /// </summary>
    public class DiceRollerCommandHandler : LocatedServiceBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizationService"></param>
        public DiceRollerCommandHandler(LocalizationService localizationService)
            : base(localizationService) { }

        #endregion //Constructor

        #region Methods

        /// <summary>
        /// Rolling given dice
        /// </summary>
        /// <param name="context"></param>
        /// <param name="diceCount"></param>
        /// <param name="diceType"></param>
        /// <returns></returns>
        public async Task Roll(InteractionContextContainer context, int diceCount, int diceType)
        {
            if (diceCount > 0)
            {
                //Variables for Rolling and Assembling the random numbers
                var builder = new System.Text.StringBuilder();
                var rnd = new Random(DateTime.Now.Nanosecond);
                var sum = 0;
                //Building the title
                var title = "Dice for " + Format.Bold(diceCount.ToString() + "d" + diceType.ToString());
                builder.AppendLine(title);

                //Rolling our dice
                for (var i = 0; i < diceCount; i++)
                {
                    var tempcounter = i + 1;
                    var tempnumber = rnd.Next(diceType) + 1;
                    sum += tempnumber;

                    var tempstring = "#" + tempcounter + ": " + Format.Bold(tempnumber.ToString());
                    builder.AppendLine(tempstring);
                }

                //Showing the sum of our roll
                var sumString = Environment.NewLine + Format.Bold("sum") + Environment.NewLine + "total = " + Format.Bold(sum.ToString());
                builder.AppendLine(sumString);

                /* Hier wird wahrscheinlich noch mit EmbedBuilder getestet
                var messagebuilder = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Description = "Dice for "
                };
                */
                await context.ReplyAsync(builder.ToString()).ConfigureAwait(false);
            }

        }


        /// <summary>
        /// Rolling given dice with a specified modifier
        /// </summary>
        /// <param name="context"></param>
        /// <param name="diceCount"></param>
        /// <param name="diceType"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public async Task RollMod(InteractionContextContainer context, int diceCount, int diceType, int modifier)
        {
            if (diceCount > 0)
            {
                var builder = new System.Text.StringBuilder();
                var rnd = new Random(DateTime.Now.Nanosecond);
                var sum = 0;
                //Building the title
                var title = "Sum of " + Format.Bold(diceCount.ToString() + "d" + diceType.ToString() + " + " + modifier.ToString());
                builder.AppendLine(title);

                //Rolling our dice
                for (var i = 0; i < diceCount; i++)
                {
                    var tempcounter = i + 1;
                    var tempnumber = rnd.Next(diceType) + 1;
                    sum += tempnumber;

                    var tempstring = "#" + tempcounter + ": " + Format.Bold(tempnumber.ToString());
                    builder.AppendLine(tempstring);
                }

                sum += modifier;
                //Showing the sum of our roll
                var sumString = Environment.NewLine + Format.Bold("sum" + " + " + modifier.ToString()) + Environment.NewLine + "total = " + Format.Bold(sum.ToString());
                builder.AppendLine(sumString);

                await context.ReplyAsync(builder.ToString()).ConfigureAwait(false);
            }
        }

        #endregion //Methods

    }
}
