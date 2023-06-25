using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

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
    /// Refresh campaign overview message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaigns/refreshMessage")]
    public async Task<IActionResult> RefreshCampaignMessage([FromBody] RefreshCampaignMessageDTO dto)
    {
        var session = await _connector.PenAndPaper
                                      .GetCampaignOverview(dto.ChannelId)
                                      .ConfigureAwait(false);

        if (await _discordClient.Client.GetChannelAsync(dto.ChannelId).ConfigureAwait(false) is ITextChannel channel)
        {
            if (await channel.GetMessageAsync(session.MessageId).ConfigureAwait(false) is IUserMessage message)
            {
                var players = new StringBuilder();

                if (session.Players?.Count > 0)
                {
                    foreach (var player in session.Players)
                    {
                        players.Append("> ");
                        players.Append(_discordClient.Client.GetUser(player.UserId).Mention);
                        players.Append(' ');

                        if (player.Class != null)
                        {
                            players.Append(DiscordEmoteService.GetEmote(_discordClient.Client, player.Class.ToString()));
                            players.Append(' ');
                        }

                        if (player.CharacterName != null)
                        {
                            players.Append(player.CharacterName);
                        }

                        players.Append(Environment.NewLine);
                    }
                }

                if (players.Length == 0)
                {
                    players.Append("> \u200b");
                }

                var embed = new EmbedBuilder().WithTitle(session.Name)
                                              .WithDescription(LocalizationGroup.GetFormattedText("Description", "{0}\n\n**DM:** <@{1}>", session.Description, session.DungeonMasterUserId))
                                              .AddField(LocalizationGroup.GetText("Players", "Players"), players.ToString())
                                              .WithTimestamp(DateTimeOffset.Now)
                                              .WithColor(Color.DarkGreen)
                                              .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64");

                var components = new ComponentBuilder().WithButton(LocalizationGroup.GetText("Settings", "⚙️"), "pnp;campaign;settings", ButtonStyle.Secondary)
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

    /// <summary>
    /// Add players
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Campaigns/addPlayers")]
    public async Task<IActionResult> AddUsersToThread([FromBody] AddPlayersDTO dto)
    {
        if (await _discordClient.Client
                                .GetChannelAsync(dto.OverviewThreadId)
                                .ConfigureAwait(false)
                is IThreadChannel overviewThread
         && await _discordClient.Client
                                .GetChannelAsync(dto.LogThreadId)
                                .ConfigureAwait(false)
                is IThreadChannel logThread)
        {
            foreach (var userId in dto.Players)
            {
                var member = await overviewThread.Guild
                                                 .GetUserAsync(userId)
                                                 .ConfigureAwait(false);

                if (member != null)
                {
                    await overviewThread.AddUserAsync(member)
                                        .ConfigureAwait(false);

                    await logThread.AddUserAsync(member)
                                   .ConfigureAwait(false);
                }
            }
        }

        return Ok();
    }

    /// <summary>
    /// Refresh session message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Sessions/refreshMessage")]
    public async Task<IActionResult> RefreshSessionMessage([FromBody] RefreshSessionMessageDTO dto)
    {
        var session = await _connector.PenAndPaper
                                      .GetSession(dto.MessageId)
                                      .ConfigureAwait(false);

        if (await _discordClient.Client.GetChannelAsync(session.ChannelId).ConfigureAwait(false) is ITextChannel channel)
        {
            if (await channel.GetMessageAsync(dto.MessageId).ConfigureAwait(false) is IUserMessage message)
            {
                var registrations = new StringBuilder();

                if (session.Registrations?.Count > 0)
                {
                    foreach (var registration in session.Registrations)
                    {
                        registrations.AppendLine($"> {(registration.IsRegistered ? DiscordEmoteService.GetCheckEmote(_discordClient.Client) : DiscordEmoteService.GetCrossEmote(_discordClient.Client))} <@{registration.UserId}>");
                    }
                }

                if (registrations.Length == 0)
                {
                    registrations.Append("> \u200b");
                }

                var embed = new EmbedBuilder().WithTitle(LocalizationGroup.GetFormattedText("SessionTitle", "Session {0:g}", session.TimeStamp))
                                              .AddField(LocalizationGroup.GetText("Registrations", "Registrations"), registrations.ToString())
                                              .WithTimestamp(DateTimeOffset.Now)
                                              .WithColor(Color.DarkGreen)
                                              .WithFooter("Devi", "https://cdn.discordapp.com/app-icons/1105924117674340423/711de34b2db8c85c927b7f709bb73b78.png?size=64");

                var components = new ComponentBuilder().WithButton(LocalizationGroup.GetText("Join", "Join"), "pnp;session;join", ButtonStyle.Secondary)
                                                       .WithButton(LocalizationGroup.GetText("Leave", "Leave"), "pnp;session;leave", ButtonStyle.Secondary)
                                                       .WithButton(LocalizationGroup.GetText("Settings", "⚙️"), "pnp;session;settings", ButtonStyle.Secondary);

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

    /// <summary>
    /// Post log message
    /// </summary>
    /// <param name="channelId">Log channel ID</param>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("Log")]
    public async Task<IActionResult> RefreshSessionMessage([FromQuery] ulong channelId, [FromBody] PostLogMessageDTO dto)
    {
        var success = true;

        if (await _discordClient.Client
                                .GetChannelAsync(channelId)
                                .ConfigureAwait(false)
            is ITextChannel logChannel)
        {
            switch (dto.Type)
            {
                case LogMessageType.SessionCreated:
                    {
                        var data = dto.Content.Deserialize<SessionCreatedDTO>();

                        await logChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("SessionCreatedLogEntry",
                                                                                             "A new session ({0}) has been created for the <t:{1}:d> at <t:{1}:t>.",
                                                                                             $"https://discord.com/channels/{logChannel.GuildId}/{data.ChannelId}/{data.MessageId}",
                                                                                             new DateTimeOffset(data.TimeStamp).ToUnixTimeSeconds()))
                                        .ConfigureAwait(false);
                    }
                    break;

                case LogMessageType.SessionDeleted:
                    {
                        var data = dto.Content.Deserialize<SessionDeletedDTO>();

                        await logChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("SessionDeletedLogEntry",
                                                                                             "The session for <t:{0}:d> at <t:{0}:t> has been deleted.",
                                                                                             new DateTimeOffset(data.TimeStamp).ToUnixTimeSeconds()))
                                        .ConfigureAwait(false);
                    }
                    break;

                case LogMessageType.UserJoined:
                    {
                        var data = dto.Content.Deserialize<UserJoinedDTO>();

                        await logChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("UserJoinedLogEntry",
                                                                                             "<@{0}> joined the session for <t:{1}:d> at <t:{1}:t>.",
                                                                                             data.UserId,
                                                                                             new DateTimeOffset(data.SessionTimeStamp).ToUnixTimeSeconds()),
                                                          allowedMentions: AllowedMentions.None)
                                        .ConfigureAwait(false);
                    }
                    break;

                case LogMessageType.UserLeft:
                    {
                        var data = dto.Content.Deserialize<UserLeftDTO>();

                        await logChannel.SendMessageAsync(LocalizationGroup.GetFormattedText("UserLeftLogEntry",
                                                                                             "<@{0}> joined the session for <t:{1}:d> at <t:{1}:t>.",
                                                                                             data.UserId,
                                                                                             new DateTimeOffset(data.SessionTimeStamp).ToUnixTimeSeconds()),
                                                          allowedMentions: AllowedMentions.None)
                                        .ConfigureAwait(false);
                    }
                    break;

                default:
                    {
                        success = false;
                    }
                    break;
            }
        }

        return success
                   ? Ok()
                   : BadRequest();
    }

    #endregion // Methods
}