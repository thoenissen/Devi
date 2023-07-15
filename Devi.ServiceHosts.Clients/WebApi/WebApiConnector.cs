using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Clients.Base;
using Devi.ServiceHosts.DTOs.Docker;
using Devi.ServiceHosts.DTOs.LookingForGroup;
using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;
using Devi.ServiceHosts.DTOs.Reminders;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Clients.WebApi;

/// <summary>
/// WebApi connector
/// </summary>
[Injectable<WebApiConnector>(ServiceLifetime.Singleton)]
public sealed class WebApiConnector : ConnectorBase,
                                      IRemindersConnector,
                                      IDockerConnector,
                                      IPenAndPaperConnector,
                                      ILookingForGroupConnector
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientFactory">Client factory</param>
    public WebApiConnector(IHttpClientFactory clientFactory)
        : base(clientFactory, Environment.GetEnvironmentVariable("DEVI_WEBAPI_BASE_URL"), true)
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

    /// <summary>
    /// Looking for group
    /// </summary>
    public ILookingForGroupConnector LookingForGroup => this;

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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.CreateCampaign(CreateCampaignDTO dto) => Post("PenAndPaper/Campaigns", dto);

    /// <summary>
    /// Create session
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="timeStamp">Time stamp</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.CreateSession(ulong channelId, ulong messageId, DateTime timeStamp) => Post("PenAndPaper/Sessions",
                                                                                                           new CreateSessionDTO
                                                                                                           {
                                                                                                               ChannelId = channelId,
                                                                                                               MessageId = messageId,
                                                                                                               TimeStamp = timeStamp
                                                                                                           });

    /// <summary>
    /// Join session
    /// </summary>
    /// <param name="dto">Join data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.JoinSession(JoinSessionDTO dto) => Post("PenAndPaper/Sessions/Registration", dto);

    /// <summary>
    /// Leave session
    /// </summary>
    /// <param name="dto">Leave data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.LeaveSession(LeaveSessionDTO dto) => Delete("PenAndPaper/Sessions/Registration", dto);

    /// <summary>
    /// Get campaign overview
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<CampaignOverviewDTO> IPenAndPaperConnector.GetCampaignOverview(ulong channelId) => Get<CampaignOverviewDTO>($"PenAndPaper/Campaigns/{channelId}/Overview");

    /// <summary>
    /// Is the given user Dungeon Master of the campaign?
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    async Task<bool> IPenAndPaperConnector.IsDungeonMaster(ulong channelId, ulong userId)
    {
        try
        {
            await Get($"PenAndPaper/Campaigns/{channelId}/IsDungeonMaster/{userId}").ConfigureAwait(false);

            return true;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    /// Set players of campaign
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="players">Players</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.SetPlayers(ulong channelId, List<ulong> players) => Post("PenAndPaper/Campaigns/Players",
                                                                                        new SetPlayersDTO
                                                                                        {
                                                                                            ChannelId = channelId,
                                                                                            Players = players
                                                                                        });

    /// <summary>
    /// Add character
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="characterName">Character name</param>
    /// <param name="characterClass">Character class</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.AddCharacter(ulong channelId, ulong userId, string characterName, Class characterClass) => Post("PenAndPaper/Campaigns/Characters",
                                                                                                                               new AddCharacterDTO
                                                                                                                               {
                                                                                                                                   ChannelId = channelId,
                                                                                                                                   UserId = userId,
                                                                                                                                   CharacterName = characterName,
                                                                                                                                   CharacterClass = characterClass
                                                                                                                               });

    /// <summary>
    /// Remove character
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.RemoveCharacter(ulong channelId, ulong userId) => Delete("PenAndPaper/Campaigns/Characters",
                                                                                        new AddCharacterDTO
                                                                                        {
                                                                                            ChannelId = channelId,
                                                                                            UserId = userId
                                                                                        });

    /// <summary>
    /// Get session
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<SessionDTO> IPenAndPaperConnector.GetSession(ulong messageId) => Get<SessionDTO>($"PenAndPaper/Sessions/{messageId}");

    /// <summary>
    /// Delete session
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task IPenAndPaperConnector.DeleteSession(ulong messageId) => Delete($"PenAndPaper/Sessions/{messageId}");

    #endregion // IPenAndPaperConnector

    #region ILookingForGroupConnector

    /// <summary>
    /// Create appointment
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ILookingForGroupConnector.CreateAppointment(CreateAppointmentDTO dto) => Post("LookingForGroup/Appointments", dto);

    /// <summary>
    /// Add registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> ILookingForGroupConnector.AddRegistration(AddRegistrationDTO dto) => Post<AddRegistrationDTO, AppointmentDTO>("LookingForGroup/Registrations", dto);

    /// <summary>
    /// Remove registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> ILookingForGroupConnector.RemoveRegistration(RemoveRegistrationDTO dto) => Delete<RemoveRegistrationDTO, AppointmentDTO>("LookingForGroup/Registrations", dto);

    /// <summary>
    /// Get appointment details
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDetailsDTO> ILookingForGroupConnector.GetAppointment(ulong appointmentMessageId) => Get<AppointmentDetailsDTO>("LookingForGroup/Appointments",
                                                                                                                                   new NameValueCollection
                                                                                                                                   {
                                                                                                                                       ["appointmentMessageId"] = appointmentMessageId.ToString()
                                                                                                                                   });

    /// <summary>
    /// Refresh appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> ILookingForGroupConnector.RefreshAppointment(ulong appointmentMessageId, RefreshAppointmentDTO dto) => Put<RefreshAppointmentDTO, AppointmentDTO>("LookingForGroup/Appointments",
                                                                                                                                                                           dto,
                                                                                                                                                                           new NameValueCollection
                                                                                                                                                                           {
                                                                                                                                                                               ["appointmentMessageId"] = appointmentMessageId.ToString()
                                                                                                                                                                           });

    /// <summary>
    /// Is the given user the creator of the appointment?
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> ILookingForGroupConnector.IsCreator(ulong appointmentMessageId, ulong userId) => Get<bool>("LookingForGroup/Appointments/isCreator",
                                                                                                          new NameValueCollection
                                                                                                          {
                                                                                                              ["appointmentMessageId"] = appointmentMessageId.ToString(),
                                                                                                              ["userId"] = userId.ToString()
                                                                                                          });

    /// <summary>
    /// Delete appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> ILookingForGroupConnector.DeleteAppointment(ulong appointmentMessageId) => Delete<Void, AppointmentDTO>("LookingForGroup/Appointments",
                                                                                                                                 null,
                                                                                                                                 new NameValueCollection
                                                                                                                                 {
                                                                                                                                     ["appointmentMessageId"] = appointmentMessageId.ToString()
                                                                                                                                 });

    #endregion // ILookingForGroupConnector
}