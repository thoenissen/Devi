using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Dialog.Base;
using Devi.ServiceHosts.Discord.Extensions;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Dialog.PenAndPaper;

/// <summary>
/// Configuration selection
/// </summary>
[Injectable<CampaignPlayerSelectionDialogElement>(ServiceLifetime.Transient)]
public class CampaignPlayerSelectionDialogElement : DialogEmbedMultiSelectSelectMenuElementBase<ulong>
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    public CampaignPlayerSelectionDialogElement(LocalizationService localizationService)
        : base(localizationService)
    {
    }

    #endregion // Constructor

    #region DialogEmbedSelectMenuElementBase<bool>

    /// <summary>
    /// Max values
    /// </summary>
    protected override int MaxValues => 10;

    /// <summary>
    /// Return the message of element
    /// </summary>
    /// <returns>Message</returns>
    public override Task<EmbedBuilder> GetMessage()
    {
        return Task.FromResult(new EmbedBuilder().WithTitle(LocalizationGroup.GetText("Title", "User selection configuration"))
                                                 .WithDescription(LocalizationGroup.GetText("Description", "Select all user which should be assigned to your campaign."))
                                                 .WithTimestamp(DateTimeOffset.Now)
                                                 .WithColor(Color.DarkGreen)
                                                 .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64"));
    }

    /// <summary>
    /// Returns the select menu entries which should be added to the message
    /// </summary>
    /// <returns>Reactions</returns>
    public override async Task<IReadOnlyList<SelectMenuOptionData>> GetEntries()
    {
        var channel = CommandContext.Channel;

        if (channel is SocketThreadChannel { ParentChannel: IMessageChannel messageChannel })
        {
            channel = messageChannel;
        }

        var channelUsers = await channel.GetUsersAsync()
                                 .Flatten()
                                 .ToListAsync()
                                 .ConfigureAwait(false);

        var options = new List<SelectMenuOptionData>();

        foreach (var user in channelUsers.Where(obj => obj.Id != CommandContext.User.Id))
        {
            var member = await CommandContext.Guild
                                             .GetUserAsync(user.Id)
                                             .ConfigureAwait(false);

            options.Add(new SelectMenuOptionData
                        {
                            Label = member.TryGetDisplayName(),
                            Value = member.Id.ToString(),
                        });
        }

        return options;
    }

    #endregion // DialogEmbedSelectMenuElementBase<bool>
}