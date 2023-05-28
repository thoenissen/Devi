using System;
using System.Text;
using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;

using Discord;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Gold Splitter commands
/// </summary>
public class UtilityCommandHandler : LocatedServiceBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public UtilityCommandHandler(LocalizationService localizationService)
        : base(localizationService) { }

    #endregion //Constructor

    #region Methods

    /// <summary>
    /// Splitting gold, silver and copper between players
    /// </summary>
    /// <param name="goldCount">Gold count</param>
    /// <param name="silverCount">Silver count</param>
    /// <param name="copperCount">Copper count</param>
    /// <param name="playerCount">Player count</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task SplitCoins(InteractionContextContainer context, int goldCount, int silverCount, int copperCount, int playerCount)
    {
        var coins = (goldCount * 100)
                  + (silverCount * 10)
                  + copperCount;

        var embed = new EmbedBuilder();

        embed.WithTitle(LocalizationGroup.GetText("CoinSplitterTitle", "Coin splitter"));

        var sb = new StringBuilder();

        sb.AppendLine(LocalizationGroup.GetFormattedText("CoinSplitterPlayerCount", "Players: {0}", playerCount));
        sb.AppendLine(LocalizationGroup.GetFormattedText("CoinSplitterCoinCount",
                                                         "Coins: {0} {1} {2} {3} {4} {5}",
                                                         coins / 100,
                                                         DiscordEmoteService.GetGoldEmote(context.Client),
                                                         coins % 100 / 10,
                                                         DiscordEmoteService.GetSilverEmote(context.Client),
                                                         coins % 10,
                                                         DiscordEmoteService.GetCopperEmote(context.Client)));

        embed.WithDescription(sb.ToString())
             .WithColor(Color.DarkBlue)
             .WithFooter(LocalizationGroup.GetText("CoinSplitterApproval", "Approved by Khrela!"), "https://cdn.discordapp.com/attachments/1111028091784019990/1112338614274228284/Khrela_portrait.png")
             .WithColor(Color.Green)
             .WithTimestamp(DateTime.Now);

        embed.AddField(LocalizationGroup.GetText("CoinSplitterSplittedCoins", "Splitted coins"), "\u200b");

        int? goldSplitted = null;
        int? silverSplitted = null;
        int? copperSplitted = null;
        int? leftoverCopper = null;

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

        if (goldSplitted != null)
        {
            embed.AddField(LocalizationGroup.GetFormattedText("CoinSplitterGold",
                                                              "{0} Gold",
                                                              DiscordEmoteService.GetGoldEmote(context.Client)),
                           goldSplitted,
                           true);
        }

        if (silverSplitted != null)
        {
            embed.AddField(LocalizationGroup.GetFormattedText("CoinSplitterSilver",
                                                              "{0} Silver",
                                                              DiscordEmoteService.GetSilverEmote(context.Client)),
                           silverSplitted,
                           true);
        }

        if (copperSplitted != null)
        {
            embed.AddField(LocalizationGroup.GetFormattedText("CoinSplitterCopper",
                                                              "{0} Copper",
                                                              DiscordEmoteService.GetCopperEmote(context.Client)),
                           copperSplitted,
                           true);
        }

        if (leftoverCopper != null)
        {
            embed.AddField("\u200b", LocalizationGroup.GetFormattedText("CoinSplitterLeftOver", "There are {0} {1} left.", leftoverCopper, DiscordEmoteService.GetCopperEmote(context.Client)));
        }

        await context.ReplyAsync(embed: embed.Build()).ConfigureAwait(false);
    }

    #endregion //Methods
}