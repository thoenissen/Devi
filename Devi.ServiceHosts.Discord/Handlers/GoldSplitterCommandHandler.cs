using System;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

using Discord;

namespace Devi.ServiceHosts.Discord.Handlers
{
    /// <summary>
    /// Gold Splitter commands
    /// </summary>
    public class GoldSplitterCommandHandler : LocatedServiceBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizationService"></param>
        public GoldSplitterCommandHandler(LocalizationService localizationService)
            : base(localizationService) { }

        #endregion //Constructor

        #region Methods
        /// <summary>
        /// A method to split Currencies between a certain number of individuals
        /// </summary>
        /// <param name="context"></param>
        /// <param name="goldCount"></param>
        /// <param name="silverCount"></param>
        /// <param name="copperCount"></param>
        /// <param name="playerCount"></param>
        /// <returns></returns>
        public async Task Split(InteractionContextContainer context, int goldCount, int silverCount, int copperCount, int playerCount)
        {
            if (playerCount > 0)
            {
                // Initializing the final currency count variables
                var goldSplitted = 0;
                var silverSplitted = 0;
                var copperSplitted = 0;
                var leftoverCopper = 0;

                // Initializing the String Builder
                var builder = new System.Text.StringBuilder();

                // Writing the title
                var title = "Currencies to be split"
                            + Environment.NewLine + "Gold: " + Format.Bold(goldCount.ToString()) + " Silver: " + Format.Bold(silverCount.ToString()) + " Copper: " + Format.Bold(copperCount.ToString())
                            + Environment.NewLine + "For " + Format.Bold(playerCount.ToString() + " Players");

                builder.AppendLine(title);
                //Splitting the currencies
                if (goldCount > 0)
                {
                    goldSplitted = goldCount / playerCount;
                    silverCount += goldCount % playerCount * 10;
                }

                if (silverCount > 0)
                {
                    silverSplitted = silverCount / playerCount;
                    copperCount += silverCount % playerCount * 10;
                }

                if (copperCount > 0)
                {
                    copperSplitted = copperCount / playerCount;
                    leftoverCopper = copperCount % playerCount;
                }

                var summary = Environment.NewLine + "Every Player recieves:"
                            + Environment.NewLine + Format.Bold(goldSplitted.ToString() + " Gold")
                            + Environment.NewLine + Format.Bold(silverSplitted.ToString() + " Silver")
                            + Environment.NewLine + Format.Bold(copperSplitted.ToString() + " Copper")
                            + Environment.NewLine + "And there are " + Format.Bold(leftoverCopper.ToString() + " Copper ") + "pieces left.";

                builder.AppendLine(summary);

                var subtext = Environment.NewLine + "Approved by Khrela!";

                builder.AppendLine(subtext);

                await context.ReplyAsync(builder.ToString()).ConfigureAwait(false);

            }
        }

        #endregion //Methods
    }
}
