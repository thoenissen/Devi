using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Interaction.Services.Discord;

using Discord;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Interaction.Services.LookingForGroup;

/// <summary>
/// Looking for group message service
/// </summary>
[Injectable<LookingForGroupMessageService>(ServiceLifetime.Transient)]
public class LookingForGroupMessageService : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Discord client
    /// </summary>
    private readonly DiscordClient _discordClient;

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
    /// <param name="discordClient">Discord client</param>
    /// <param name="connector">Web-API</param>
    public LookingForGroupMessageService(LocalizationService localizationService,
                                         DiscordClient discordClient,
                                         WebApiConnector connector)
        : base(localizationService)
    {
        _discordClient = discordClient;
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Refresh message
    /// </summary>
    /// <param name="appointmentMessageId">Id of the appointment</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task RefreshMessage(ulong appointmentMessageId)
    {
        var appointmentData = await _connector.LookingForGroup
                                              .GetAppointment(appointmentMessageId)
                                              .ConfigureAwait(false);

        if (appointmentData != null)
        {
            var channel = await _discordClient.Client
                                              .GetChannelAsync(appointmentData.ChannelId)
                                              .ConfigureAwait(false);

            if (channel is ITextChannel textChannel)
            {
                var message = await textChannel.GetMessageAsync(appointmentMessageId)
                                               .ConfigureAwait(false);

                if (message is IUserMessage userMessage)
                {
                    var embedBuilder = new EmbedBuilder().WithTitle("LFG: " + appointmentData.Title)
                                                         .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64")
                                                         .WithColor(Color.Green)
                                                         .WithTimestamp(DateTime.Now)
                                                         .WithThumbnailUrl("https://cdn.discordapp.com/attachments/982003562462724186/1018981799759716432/unknown.png");

                    if (string.IsNullOrWhiteSpace(appointmentData.Description) == false)
                    {
                        embedBuilder.WithDescription(appointmentData.Description);
                    }

                    var participantCount = 0;
                    var participantsBuilder = new StringBuilder();

                    foreach (var participant in appointmentData.Participants.OrderBy(obj => obj.RegistrationTimeStamp))
                    {
                        var line = "> " + _discordClient.Client
                                                        .GetUser(participant.UserId).Mention;

                        if (participantCount > 0
                         && participantCount % 10 == 0)
                        {
                            embedBuilder.AddField(embedBuilder.Fields.Count == 0 ? LocalizationGroup.GetText("Participants", "Participants") : "\u200b", participantsBuilder.ToString(), true);
                            participantsBuilder = new StringBuilder();
                        }

                        participantsBuilder.AppendLine(line);
                        participantCount++;
                    }

                    if (participantsBuilder.Length == 0)
                    {
                        participantsBuilder.Append(">  \u200b");
                    }

                    embedBuilder.AddField(embedBuilder.Fields.Count == 0 ? LocalizationGroup.GetText("Participants", "Participants") : "\u200b", participantsBuilder.ToString(), true);

                    var componentsBuilder = new ComponentBuilder();

                    componentsBuilder.WithButton(LocalizationGroup.GetText("Join", "Join"),
                                                 InteractivityService.GetPermanentCustomId("lfg", "join"),
                                                 ButtonStyle.Secondary,
                                                 DiscordEmoteService.GetCheckEmote(_discordClient.Client));
                    componentsBuilder.WithButton(LocalizationGroup.GetText("Leave", "Leave"),
                                                 InteractivityService.GetPermanentCustomId("lfg", "leave"),
                                                 ButtonStyle.Secondary,
                                                 DiscordEmoteService.GetCrossEmote(_discordClient.Client));
                    componentsBuilder.WithButton(LocalizationGroup.GetText("Thread", "Thread"),
                                                 style: ButtonStyle.Link,
                                                 url: $"https://discord.com/channels/{textChannel.GuildId}/{appointmentData.ThreadId}/");
                    componentsBuilder.WithButton(null,
                                                 InteractivityService.GetPermanentCustomId("lfg", "configuration"),
                                                 ButtonStyle.Secondary,
                                                 new Emoji("⚙️"));

                    await userMessage.ModifyAsync(obj =>
                                                  {
                                                      obj.Content = string.Empty;
                                                      obj.Embed = embedBuilder.Build();
                                                      obj.Components = componentsBuilder.Build();
                                                  })
                                     .ConfigureAwait(false);
                }
            }
        }
    }

    #endregion // Methods
}