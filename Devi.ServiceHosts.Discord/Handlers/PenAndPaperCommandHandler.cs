using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;

using Discord;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Dungeon master commands
/// </summary>
public class PenAndPaperCommandHandler : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Connector</param>
    public PenAndPaperCommandHandler(LocalizationService localizationService, WebApiConnector connector)
        : base(localizationService)
    {
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CreateCampaign(InteractionContextContainer context)
    {
        await context.RespondWithModalAsync<CreateCampaignModalData>("modal;pnp;campaign;create")
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CreateCampaign(InteractionContextContainer context, CreateCampaignModalData data)
    {
        DayOfWeek? selectedDay = null;

        foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>())
        {
            if (LocalizationGroup.CultureInfo.DateTimeFormat.GetDayName(dayOfWeek).StartsWith(data.Day))
            {
                selectedDay = dayOfWeek;
                break;
            }
        }

        if (selectedDay != null
         && TimeSpan.TryParseExact(data.Time, "hh\\:mm", LocalizationGroup.CultureInfo, out var selectedTime))
        {
            if (context.Channel is ITextChannel textChannel)
            {
                await context.DeferAsync()
                             .ConfigureAwait(false);

                var message = await textChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("CampaignCreationLoading", "{0} The campaign is being created.", DiscordEmoteService.GetLoadingEmote(context.Client)))
                                               .ConfigureAwait(false);

                await message.PinAsync()
                             .ConfigureAwait(false);

                var thread = await textChannel.CreateThreadAsync(LocalizationGroup.GetText("CampaignCreationLogThread", "Log"), ThreadType.PublicThread, ThreadArchiveDuration.OneWeek)
                                              .ConfigureAwait(false);

                await thread.AddUserAsync(context.Member)
                            .ConfigureAwait(false);

                await textChannel.DeleteMessageAsync(thread.Id)
                                 .ConfigureAwait(false);

                _connector.PenAndPaper.CreateCampaign(new CreateCampaignDTO
                                                      {
                                                          Name = data.Name,
                                                          Description = data.Description,
                                                          ChannelId = textChannel.Id,
                                                          MessageId = message.Id,
                                                          ThreadId = thread.Id,
                                                          DungeonMasterUserId = context.User.Id,
                                                          DayOfWeek = selectedDay,
                                                          Time = selectedTime
                                                      });
            }
        }
    }

    #endregion // Methods
}