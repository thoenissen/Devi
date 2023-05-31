﻿using System;
using System.Text;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;

using Discord;

using Microsoft.AspNetCore.Mvc;

namespace Devi.ServiceHosts.Discord.Controller;

/// <summary>
/// Dungeon master controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class PenAndPaperController : LocatedControllerBase
{
    #region Fields

    /// <summary>
    /// Discord client
    /// </summary>
    private readonly DiscordClient _discordClient;

    /// <summary>
    /// Web API connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="discordClient">Discord client</param>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Web API connector</param>
    public PenAndPaperController(DiscordClient discordClient,
                                 LocalizationService localizationService,
                                 WebApiConnector connector)
        : base(localizationService)
    {
        _discordClient = discordClient;
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Post reminder message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaign/refreshMessage")]
    public async Task<IActionResult> PostReminderMessage([FromBody] RefreshCampaignMessageDTO dto)
    {
        var session = await _connector.PenAndPaper
                                      .GetCurrentSession(dto.ChannelId)
                                      .ConfigureAwait(false);

        if (await _discordClient.Client.GetChannelAsync(dto.ChannelId).ConfigureAwait(false) is ITextChannel channel)
        {
            if (await channel.GetMessageAsync(session.MessageId).ConfigureAwait(false) is IUserMessage message)
            {
                var sessionRegistrations = new StringBuilder();

                if (session.Registrations?.Count > 0)
                {
                    foreach (var registration in session.Registrations)
                    {
                        sessionRegistrations.AppendLine($"> {(registration.IsRegistered ? DiscordEmoteService.GetCheckEmote(_discordClient.Client) : DiscordEmoteService.GetCrossEmote(_discordClient.Client))} <@{registration.UserId}>");
                    }
                }

                if (sessionRegistrations.Length == 0)
                {
                    sessionRegistrations.Append("> \u200b");
                }

                var embed = new EmbedBuilder().WithTitle(session.Name)
                                              .WithDescription(LocalizationGroup.GetFormattedText("Description", "{0}\n\n**DM:** <@{1}>", session.Description, session.DungeonMasterUserId))
                                              .AddField(LocalizationGroup.GetFormattedText("Session", "Session {0:g}", session.SessionTimeStamp), sessionRegistrations.ToString())
                                              .WithTimestamp(DateTimeOffset.Now)
                                              .WithColor(Color.DarkGreen)
                                              .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64");

                var components = new ComponentBuilder().WithButton(LocalizationGroup.GetText("Join", "Join"), "pnp;session;join", ButtonStyle.Secondary)
                                                       .WithButton(LocalizationGroup.GetText("Leave", "Leave"), "pnp;session;leave", ButtonStyle.Secondary)
                                                       .WithButton(LocalizationGroup.GetText("Settings", "⚙️"), "pnp;campaign;settings", ButtonStyle.Secondary)
                                                       .WithButton(LocalizationGroup.GetText("Log", "Log"), null, ButtonStyle.Link, null, $"https://discord.com/channels/{channel.GuildId}/{session.ThreadId}/");

                await message.ModifyAsync(obj =>
                                          {
                                              obj.Content = null;
                                              obj.Embed = embed.Build();
                                              obj.Components = components.Build();
                                          })
                             .ConfigureAwait(false);
            }
        }

        return Ok();
    }

    #endregion // Methods
}