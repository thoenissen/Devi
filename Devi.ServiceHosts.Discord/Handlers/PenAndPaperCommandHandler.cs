using System;
using System.Linq;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Dialog.Base;
using Devi.ServiceHosts.Discord.Dialog.PenAndPaper;
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
            if (dayOfWeek.ToString().StartsWith(data.Day))
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

                var logThread = await textChannel.CreateThreadAsync(LocalizationGroup.GetText("CampaignCreationLogThread", "Log"), ThreadType.PublicThread, ThreadArchiveDuration.OneWeek)
                                                 .ConfigureAwait(false);

                await logThread.AddUserAsync(context.Member)
                               .ConfigureAwait(false);

                await textChannel.DeleteMessageAsync(logThread.Id)
                                 .ConfigureAwait(false);

                var overviewThread = await textChannel.CreateThreadAsync(LocalizationGroup.GetText("CampaignCreationOverviewThread", "Overview"), ThreadType.PublicThread, ThreadArchiveDuration.OneWeek)
                                                      .ConfigureAwait(false);

                var message = await overviewThread.SendMessageAsync(LocalizationGroup.GetFormattedText("CampaignCreationLoading", "{0} The campaign is being created.", DiscordEmoteService.GetLoadingEmote(context.Client)))
                                                  .ConfigureAwait(false);

                await overviewThread.AddUserAsync(context.Member)
                                    .ConfigureAwait(false);

                await textChannel.DeleteMessageAsync(overviewThread.Id)
                                 .ConfigureAwait(false);

                await _connector.PenAndPaper
                                .CreateCampaign(new CreateCampaignDTO
                                                {
                                                    Name = data.Name,
                                                    Description = data.Description,
                                                    ChannelId = overviewThread.Id,
                                                    MessageId = message.Id,
                                                    ThreadId = logThread.Id,
                                                    DungeonMasterUserId = context.User.Id,
                                                    DayOfWeek = selectedDay.Value,
                                                    Time = selectedTime
                                                })
                                .ConfigureAwait(false);

                await textChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("CampaignCreationChannelsCreated", "The the campaign has been successfully created.\n\n**Threads**\n> <#{0}>\n> <#{1}>", overviewThread.Id, logThread.Id))
                                 .ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Campaign configuration
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CampaignSettings(InteractionContextContainer context)
    {
        await context.DeferProcessing(true)
                     .ConfigureAwait(false);

        if (await  _connector.PenAndPaper
                             .IsDungeonMaster(context.Channel.Id, context.User.Id)
                             .ConfigureAwait(false))
        {
            using (var dialogHandler = new DialogHandler(context))
            {
                dialogHandler.DialogContext.UseEphemeralMessages = true;
                dialogHandler.DialogContext.ModifyCurrentMessage = true;

                bool restartDialog;

                do
                {
                    restartDialog = await dialogHandler.Run<CampaignSettingsSelectionDialogElement, bool>()
                                                       .ConfigureAwait(false);
                }
                while (restartDialog);
            }

            await context.DeleteOriginalResponse()
                         .ConfigureAwait(false);
        }
        else
        {
            await context.ReplyAsync(LocalizationGroup.GetText("OnlyDungeonMasterAllowed", "Only the Dungeon Master is allowed to use the configuration."),
                                     ephemeral: true)
                         .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Session creation
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="date">Date</param>
    /// <param name="time">Time</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task CreateSession(InteractionContextContainer context, string date, string time)
    {
        if (DateTime.TryParseExact($"{date}_{time}", "dd.MM.yyyy_HH:mm", null, System.Globalization.DateTimeStyles.None, out var timeStamp))
        {
            await context.DeferProcessing(true)
                         .ConfigureAwait(false);

            var message = await context.Channel.SendMessageAsync(LocalizationGroup.GetFormattedText("SessionCreation", "{0} The session is being created.", DiscordEmoteService.GetLoadingEmote(context.Client)))
                                       .ConfigureAwait(false);

            await _connector.PenAndPaper
                            .CreateSession(context.Channel.Id, message.Id, timeStamp)
                            .ConfigureAwait(false);

            await context.DeleteOriginalResponse()
                         .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Join session
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task JoinSession(InteractionContextContainer context)
    {
        await context.DeferAsync()
                     .ConfigureAwait(false);

        await _connector.PenAndPaper
                        .JoinSession(new JoinSessionDTO
                                     {
                                         MessageId = context.Message.Id,
                                         UserId = context.User.Id
                                     })
                        .ConfigureAwait(false);
    }

    /// <summary>
    /// Leave session
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task LeaveSession(InteractionContextContainer context)
    {
        await context.DeferAsync()
                     .ConfigureAwait(false);

        await _connector.PenAndPaper
                        .LeaveSession(new LeaveSessionDTO
                                      {
                                          MessageId = context.Message.Id,
                                          UserId = context.User.Id
                                      })
                        .ConfigureAwait(false);
    }

    #endregion // Methods
}