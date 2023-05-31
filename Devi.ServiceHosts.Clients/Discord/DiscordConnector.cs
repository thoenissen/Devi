using System;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Base;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.Reminders;

namespace Devi.ServiceHosts.Clients.Discord;

/// <summary>
/// Discord connector
/// </summary>
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
    Task IPenAndPaperConnector.RefreshCampaignMessage(RefreshCampaignMessageDTO dto) => Post("PenAndPaper/Campaign/refreshMessage", dto);

    #endregion // Methods
}