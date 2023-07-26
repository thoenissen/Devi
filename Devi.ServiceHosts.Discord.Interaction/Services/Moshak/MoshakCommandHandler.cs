using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Interaction.Enumerations.Moshak;
using Devi.ServiceHosts.Discord.Interaction.Services.Discord;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Services.Moshak;

/// <summary>
/// Moshak command service
/// </summary>
[Injectable<MoashkCommandHandler>(ServiceLifetime.Transient)]
public class MoashkCommandHandler : LocatedServiceBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public MoashkCommandHandler(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Calculate time spans
    /// </summary>
    /// <param name="commandContext">Command context</param>
    /// <param name="timeSpanString">Time span string</param>
    /// <param name="dimension">Dimension</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CalculateTimeSpan(InteractionContextContainer commandContext, string timeSpanString, Dimensions dimension)
    {
        var timeSpan = TimeSpan.Zero;

        foreach (Match match in Regex.Matches(timeSpanString, @"\d+(y|d|h|m|s)"))
        {
            var amount = Convert.ToUInt64(match.Value[..^1], CultureInfo.InvariantCulture);

            timeSpan += match.Value[^1..] switch
                        {
                            "y" => TimeSpan.FromDays(amount * 365),
                            "d" => TimeSpan.FromDays(amount),
                            "h" => TimeSpan.FromHours(amount),
                            "m" => TimeSpan.FromMinutes(amount),
                            "s" => TimeSpan.FromSeconds(amount),
                            _ => throw new InvalidOperationException()
                        };
        }

        var moshakTimeSpan = dimension switch
                             {
                                 Dimensions.Shadeward => timeSpan * 100,
                                 Dimensions.Moshak => timeSpan,
                                 Dimensions.Feywild => timeSpan / 100,
                                 _ => throw new InvalidOperationException()
                             };

        static string FormatTimeSpan(TimeSpan timeSpan)
        {
            var builder = new StringBuilder();

            if (timeSpan.Days > 365)
            {
                var years = timeSpan.Days / 365;

                builder.Append(years);
                builder.Append(" year");

                if (years != 1)
                {
                    builder.Append('s');
                }

                builder.Append(' ');
            }

            var days = timeSpan.Days % 365;

            if (days > 0)
            {
                builder.Append(days);
                builder.Append(" day");

                if (days != 1)
                {
                    builder.Append('s');
                }

                builder.Append(' ');
            }

            if (timeSpan.Hours > 0
             || timeSpan.Minutes > 0
             || timeSpan.Seconds > 0)
            {
                builder.Append(timeSpan.Hours);
                builder.Append(" hour");

                if (timeSpan.Hours != 1)
                {
                    builder.Append('s');
                }

                builder.Append(' ');
            }

            if (timeSpan.Minutes > 0
             || timeSpan.Seconds > 0)
            {
                builder.Append(timeSpan.Minutes);
                builder.Append(" minute");

                if (timeSpan.Minutes != 1)
                {
                    builder.Append('s');
                }

                builder.Append(' ');
            }

            if (timeSpan.Seconds > 0)
            {
                builder.Append(timeSpan.Seconds);
                builder.Append(" second");

                if (timeSpan.Seconds != 1)
                {
                    builder.Append('s');
                }
            }

            return builder.ToString();
        }

        var embed = new EmbedBuilder().WithTitle(LocalizationGroup.GetText("TimeSpanCalculationTitle", "Interdimensional time span calculation"))
                                      .WithDescription(LocalizationGroup.GetFormattedText("TimeSpanCalculationDescription", "The given time span is translated into the equivalent time span of the other dimensions."))
                                      .AddField("Shadeward", FormatTimeSpan(moshakTimeSpan / 100))
                                      .AddField("Moshak", FormatTimeSpan(moshakTimeSpan))
                                      .AddField("Feywild", FormatTimeSpan(moshakTimeSpan * 100))
                                      .WithTimestamp(DateTimeOffset.Now)
                                      .WithColor(Color.DarkGreen)
                                      .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64");

        await commandContext.ReplyAsync(embed: embed.Build())
                            .ConfigureAwait(false);
    }

    #endregion // Methods
}