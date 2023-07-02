﻿using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.Base;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.Reminders;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Clients.Discord;

/// <summary>
/// Discord connector
/// </summary>
[Injectable<DiscordConnector>(ServiceLifetime.Singleton)]
[Injectable<IRemindersConnector>(ServiceLifetime.Singleton)]
[Injectable<IPenAndPaperConnector>(ServiceLifetime.Singleton)]
public sealed class DiscordConnector : ConnectorBase,
                                       IRemindersConnector,
                                       IPenAndPaperConnector
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientFactory">Client factory</param>
    public DiscordConnector(IHttpClientFactory clientFactory)
        : base(clientFactory, Environment.GetEnvironmentVariable("DEVI_DISCORD_BASE_URL"))
    {
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Reminders
    /// </summary>
    public IRemindersConnector Reminders => this;

    /// <summary>
    /// Pen and paper
    /// </summary>
    public IPenAndPaperConnector PenAndPaper => this;

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task IRemindersConnector.PostOneTimeReminder(PostReminderMessageDTO dto) => Post("reminders", dto);

    /// <summary>
    /// Refresh campaign message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task IPenAndPaperConnector.RefreshCampaignMessage(RefreshCampaignMessageDTO dto) => Post("PenAndPaper/Campaigns/refreshMessage", dto);

    /// <summary>
    /// Refresh session message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task IPenAndPaperConnector.RefreshSessionMessage(RefreshSessionMessageDTO dto) => Post("PenAndPaper/Sessions/refreshMessage", dto);

    /// <summary>
    /// Post log message
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <param name="channelId">Channel ID</param>
    /// <typeparam name="T">Sub type</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task PostLogMessage<T>(PostLogMessageDTO<T> dto, ulong channelId) => Post("PenAndPaper/Log",
                                                                                     dto,
                                                                                     new NameValueCollection
                                                                                     {
                                                                                         ["channelId"] = channelId.ToString()
                                                                                     });

    /// <summary>
    /// Add players to threads
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddPlayers(AddPlayersDTO dto) => Post("PenAndPaper/Campaigns/addPlayers", dto);

    #endregion // Methods
}