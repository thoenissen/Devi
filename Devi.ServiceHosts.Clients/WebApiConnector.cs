using System;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.Reminders;

namespace Devi.ServiceHosts.Clients;

/// <summary>
/// WebApi connector
/// </summary>
public class WebApiConnector : ConnectorBase
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientFactory">Client factory</param>
    public WebApiConnector(IHttpClientFactory clientFactory)
        : base(clientFactory, Environment.GetEnvironmentVariable("DEVI_WEBAPI_BASE_URL"))
    {
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task CreateOneTimeReminder(CreateOneTimeReminderDTO dto) => Post("reminders", dto);

    #endregion // Methods
}