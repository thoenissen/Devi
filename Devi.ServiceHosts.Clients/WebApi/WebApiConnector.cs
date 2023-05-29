using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.Base;
using Devi.ServiceHosts.DTOs.Docker;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.Reminders;

namespace Devi.ServiceHosts.Clients.WebApi;

/// <summary>
/// WebApi connector
/// </summary>
public sealed class WebApiConnector : ConnectorBase,
                                      IRemindersConnector,
                                      IDockerConnector,
                                      IPenAndPaperConnector
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

    #region Properties

    /// <summary>
    /// Reminders
    /// </summary>
    public IRemindersConnector Reminders => this;

    /// <summary>
    /// Docker
    /// </summary>
    public IDockerConnector Docker => this;

    /// <summary>
    /// Pen and paper
    /// </summary>
    public IPenAndPaperConnector PenAndPaper => this;

    #endregion // Properties

    #region IRemindersConnector

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task IRemindersConnector.CreateOneTimeReminder(CreateOneTimeReminderDTO dto) => Post("Reminders", dto);

    #endregion // IRemindersConnector

    #region IDockerConnector

    /// <summary>
    /// Get docker containers
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<List<DockerContainerDTO>> IDockerConnector.GetDockerContainers(ulong serverId) => Get<List<DockerContainerDTO>>("Docker/Containers",
                                                                                                                         new NameValueCollection
                                                                                                                         {
                                                                                                                             ["serverId"] = serverId.ToString(),
                                                                                                                         });

    /// <summary>
    /// Add or refresh dto
    /// </summary>
    /// <param name="serverId">Server ID</param>
    /// <param name="dto">Container</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task IDockerConnector.AddOrRefreshContainer(ulong serverId, DockerContainerDTO dto) => Put("Docker/Containers",
                                                                                               dto,
                                                                                               new NameValueCollection
                                                                                               {
                                                                                                   ["serverId"] = serverId.ToString()
                                                                                               });

    #endregion // IDockerConnector

    #region IPenAndPaperConnector

    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="dto">Campaign data</param>
    void IPenAndPaperConnector.CreateCampaign(CreateCampaignDTO dto) => Post("PenAndPaper/Campaigns", dto);

    #endregion // IPenAndPaperConnector
}