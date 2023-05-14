using System;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Reminders;

namespace Devi.ServiceHosts.Clients;

/// <summary>
/// Discord connector
/// </summary>
public class DiscordConnector : ConnectorBase
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

    #region Methods

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task PostOneTimeReminder(PostReminderMessageDTO dto) => Post("reminders", dto);

    #endregion // Methods
}