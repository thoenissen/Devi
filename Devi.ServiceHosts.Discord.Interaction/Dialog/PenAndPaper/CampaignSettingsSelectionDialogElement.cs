using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Interaction.Dialog.Base;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Dialog.PenAndPaper;

/// <summary>
/// Configuration selection
/// </summary>
[Injectable<CampaignSettingsSelectionDialogElement>(ServiceLifetime.Transient)]
public class CampaignSettingsSelectionDialogElement : DialogEmbedSelectMenuElementBase<bool>
{
    #region Fields

    /// <summary>
    /// Web API connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Web API connector</param>
    public CampaignSettingsSelectionDialogElement(LocalizationService localizationService,
                                                  WebApiConnector connector)
        : base(localizationService)
    {
        _connector = connector;
    }

    #endregion // Constructor

    #region DialogEmbedSelectMenuElementBase<bool>

    /// <summary>
    /// Return the message of element
    /// </summary>
    /// <returns>Message</returns>
    public override Task<EmbedBuilder> GetMessage()
    {
        return Task.FromResult(new EmbedBuilder().WithTitle(LocalizationGroup.GetText("Title", "Campaign configuration"))
                                                 .WithDescription(LocalizationGroup.GetText("Description", "With the following assistant you are able to configure your campaign."))
                                                 .WithTimestamp(DateTimeOffset.Now)
                                                 .WithColor(Color.DarkGreen)
                                                 .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64"));
    }

    /// <summary>
    /// Returns the select menu entries which should be added to the message
    /// </summary>
    /// <returns>Reactions</returns>
    public override IReadOnlyList<SelectMenuEntryData<bool>> GetEntries()
    {
        return new List<SelectMenuEntryData<bool>>
               {
                   new()
                   {
                       CommandText = LocalizationGroup.GetText("OptionPlayers", "Players"),
                       Response = async () =>
                                  {
                                      var users = await RunSubElement<CampaignPlayerSelectionDialogElement, List<ulong>>().ConfigureAwait(false);

                                      if (users != null)
                                      {
                                          await _connector.PenAndPaper
                                                          .SetPlayers(CommandContext.Channel.Id, users)
                                                          .ConfigureAwait(false);

                                          return false;
                                      }

                                      return true;
                                  },
                   }
               };
    }

    /// <summary>
    /// Returning the placeholder
    /// </summary>
    /// <returns>Placeholder</returns>
    public override string GetPlaceholder() => LocalizationGroup.GetText("Placeholder", "Please select a option...");

    /// <summary>
    /// Default case if none of the given buttons is used
    /// </summary>
    /// <returns>Result</returns>
    protected override bool DefaultFunc() => false;

    #endregion // DialogEmbedSelectMenuElementBase<bool>
}