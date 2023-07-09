using System;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Interaction.Commands.Modals.Data;
using Devi.ServiceHosts.Discord.Interaction.Services.Discord;
using Devi.ServiceHosts.Discord.Interaction.Services.LookingForGroup;
using Devi.ServiceHosts.DTOs.LookingForGroup;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Handlers;

/// <summary>
/// Looking for group command handler
/// </summary>
[Injectable<LookingForGroupCommandHandler>(ServiceLifetime.Transient)]
public class LookingForGroupCommandHandler : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Web-API
    /// </summary>
    private readonly WebApiConnector _connector;

    /// <summary>
    /// Message service
    /// </summary>
    private readonly LookingForGroupMessageService _messageService;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="messageService">Message service</param>
    /// <param name="connector">Web-API</param>
    public LookingForGroupCommandHandler(LocalizationService localizationService,
                                         LookingForGroupMessageService messageService,
                                         WebApiConnector connector)
        : base(localizationService)
    {
        _messageService = messageService;
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Starting the creation of an new appointment
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task StartCreation(InteractionContextContainer context)
    {
        await context.RespondWithModalAsync<LookingForGroupCreationModalData>(LookingForGroupCreationModalData.CustomId)
                     .ConfigureAwait(false);
    }

    /// <summary>
    /// Creation of an new appoint
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task Create(InteractionContextContainer context, string title, string description)
    {
        await context.DeferAsync()
                     .ConfigureAwait(false);

        if (context.Channel is ITextChannel textChannel)
        {
            var message = await textChannel.SendMessageAsync(DiscordEmoteService.GetLoadingEmote(context.Client) + " " + LocalizationGroup.GetText("AppointmentCreation", "The appointment is being created."))
                                           .ConfigureAwait(false);

            var thread = await textChannel.CreateThreadAsync("LFG: " + title)
                                          .ConfigureAwait(false);

            await thread.AddUserAsync(context.Member)
                        .ConfigureAwait(false);

            await textChannel.DeleteMessageAsync(thread.Id)
                             .ConfigureAwait(false);

            await _connector.LookingForGroup
                            .CreateAppointment(new CreateAppointmentDTO
                                               {
                                                   ChannelId = textChannel.Id,
                                                   MessageId = message.Id,
                                                   CreationUserId = context.User.Id,
                                                   Title = title,
                                                   Description = description,
                                                   ThreadId = thread.Id
                                               })
                            .ConfigureAwait(false);

            await _connector.LookingForGroup
                            .AddRegistration(new AddRegistrationDTO
                                             {
                                                 AppointmentMessageId = message.Id,
                                                 UserId = context.User.Id,
                                             })
                            .ConfigureAwait(false);

            await _messageService.RefreshMessage(message.Id)
                                 .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creation of an new appoint
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="title">Title</param>
    /// <param name="description">Description</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task Edit(InteractionContextContainer context, ulong appointmentMessageId, string title, string description)
    {
        await context.DeferAsync()
                     .ConfigureAwait(false);

        var appointmentData = await _connector.LookingForGroup
                                              .RefreshAppointment(appointmentMessageId,
                                                                  new RefreshAppointmentDTO
                                                                  {
                                                                      Title = title,
                                                                      Description = description,
                                                                  })
                                              .ConfigureAwait(false);

        await _messageService.RefreshMessage(appointmentMessageId)
                             .ConfigureAwait(false);

        if (await context.Client
                         .GetChannelAsync(appointmentData.ThreadId)
                         .ConfigureAwait(false) is IThreadChannel threadChannel)
        {
            await threadChannel.ModifyAsync(obj => obj.Name = "LFG: " + title)
                               .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Joining an appointment
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Join(InteractionContextContainer context)
    {
        var appointmentData = await _connector.LookingForGroup
                                              .AddRegistration(new AddRegistrationDTO
                                                               {
                                                                   AppointmentMessageId = context.Message.Id,
                                                                   UserId = context.User.Id,
                                                               })
                                              .ConfigureAwait(false);

        await _messageService.RefreshMessage(context.Message.Id)
                             .ConfigureAwait(false);

        if (await context.Client
                         .GetChannelAsync(appointmentData.ThreadId)
                         .ConfigureAwait(false) is IThreadChannel threadChannel)
        {
            await threadChannel.AddUserAsync(context.Member)
                               .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Leaving an appointment
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Leave(InteractionContextContainer context)
    {
        var appointmentData = await _connector.LookingForGroup
                                              .RemoveRegistration(new RemoveRegistrationDTO
                                                                  {
                                                                      AppointmentMessageId = context.Message.Id,
                                                                      UserId = context.User.Id,
                                                                  })
                                              .ConfigureAwait(false);

        await _messageService.RefreshMessage(context.Message.Id)
                             .ConfigureAwait(false);

        if (await context.Client
                         .GetChannelAsync(appointmentData.ThreadId)
                         .ConfigureAwait(false) is IThreadChannel threadChannel)
        {
            await threadChannel.RemoveUserAsync(context.Member)
                               .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Configuration an appointment
    /// </summary>
    /// <param name="context">Command context</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Configure(InteractionContextContainer context)
    {
        await context.DeferProcessing(true)
                     .ConfigureAwait(false);

        if (context.User is IGuildUser { GuildPermissions.Administrator: true }
         || await _connector.LookingForGroup
                            .IsCreator(context.Message.Id, context.User.Id)
                            .ConfigureAwait(false))
        {
            var embedBuilder = new EmbedBuilder().WithTitle(LocalizationGroup.GetText("ConfigurationTitle", "Configuration assistant"))
                                                 .WithDescription(LocalizationGroup.GetText("ConfigurationDescription", "With the following assistant you can configure the existing appoint. You can dismiss this message, if you don't want to edit anything anymore."))
                                                 .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64")
                                                 .WithColor(Color.Green)
                                                 .WithTimestamp(DateTime.Now);

            var componentsBuilder = new ComponentBuilder();

            componentsBuilder.WithSelectMenu(new SelectMenuBuilder().AddOption(LocalizationGroup.GetText("ConfigurationMenuEdit", "Edit appointment data"), "edit", emote: DiscordEmoteService.GetEditEmote(context.Client))
                                                                    .AddOption(LocalizationGroup.GetText("ConfigurationMenuDelete", "Delete appointment"), "delete", emote: DiscordEmoteService.GetTrashCanEmote(context.Client))
                                                                    .WithCustomId(InteractivityService.GetPermanentCustomId("lfg", "configureMenu", context.Message.Id.ToString())));

            await context.ModifyOriginalResponseAsync(obj =>
                                                      {
                                                          obj.Content = null;
                                                          obj.Embed = embedBuilder.Build();
                                                          obj.Components = componentsBuilder.Build();
                                                      })
                         .ConfigureAwait(false);
        }
        else
        {
            await context.ModifyOriginalResponseAsync(obj => obj.Content = LocalizationGroup.GetText("ConfigurationPermissionsDenied", "You are not allowed to edit the appointment."))
                         .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Configure menu options
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="value">Selected value</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ConfigureMenuOption(InteractionContextContainer context, ulong appointmentMessageId, string value)
    {
        if (context.User is IGuildUser { GuildPermissions.Administrator: true }
         || await _connector.LookingForGroup
                            .IsCreator(appointmentMessageId, context.User.Id)
                            .ConfigureAwait(false))
        {
            switch (value)
            {
                case "edit":
                    {
                        await context.RespondWithModalAsync<LookingForGroupEditModalData>($"{LookingForGroupEditModalData.CustomId};{appointmentMessageId}")
                                     .ConfigureAwait(false);

                        await context.DeleteOriginalResponse()
                                     .ConfigureAwait(false);
                    }
                    break;

                case "delete":
                    {
                        await context.DeferAsync()
                                     .ConfigureAwait(false);

                        await context.DeleteOriginalResponse()
                                     .ConfigureAwait(false);

                        var data = await _connector.LookingForGroup
                                                   .DeleteAppointment(appointmentMessageId)
                                                   .ConfigureAwait(false);

                        if (data != null)
                        {
                            if (await context.Client
                                             .GetChannelAsync(data.ThreadId)
                                             .ConfigureAwait(false) is IThreadChannel threadChannel)
                            {
                                await threadChannel.DeleteAsync()
                                                   .ConfigureAwait(false);
                            }

                            if (context.Channel is ITextChannel textChannel)
                            {
                                await textChannel.DeleteMessageAsync(appointmentMessageId)
                                                 .ConfigureAwait(false);
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            await context.ReplyAsync(LocalizationGroup.GetText("ConfigurationPermissionsDenied", "You are not allowed to edit the appointment."),
                                     ephemeral: true)
                         .ConfigureAwait(false);
        }
    }

    #endregion // Methods
}