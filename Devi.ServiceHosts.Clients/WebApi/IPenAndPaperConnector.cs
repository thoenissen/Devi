using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.PenAndPaper;
using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

namespace Devi.ServiceHosts.Clients.WebApi;

/// <summary>
/// Pen and paper connector
/// </summary>
public interface IPenAndPaperConnector
{
    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="dto">Campaign data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task CreateCampaign(CreateCampaignDTO dto);

    /// <summary>
    /// Create session
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="timeStamp">Time stamp</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task CreateSession(ulong channelId, ulong messageId, DateTime timeStamp);

    /// <summary>
    /// Join session
    /// </summary>
    /// <param name="dto">Join data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task JoinSession(JoinSessionDTO dto);

    /// <summary>
    /// Leave session
    /// </summary>
    /// <param name="dto">Leave data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task LeaveSession(LeaveSessionDTO dto);

    /// <summary>
    /// Get campaign overview
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<CampaignOverviewDTO> GetCampaignOverview(ulong channelId);

    /// <summary>
    /// Is the given user Dungeon Master of the campaign?
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<bool> IsDungeonMaster(ulong channelId, ulong userId);

    /// <summary>
    /// Set players of campaign
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="users">Users</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task SetPlayers(ulong channelId, List<ulong> users);

    /// <summary>
    /// Add character
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="characterName">Character name</param>
    /// <param name="characterClass">Character class</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task AddCharacter(ulong channelId, ulong userId, string characterName, Class characterClass);

    /// <summary>
    /// Remove character
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task RemoveCharacter(ulong channelId, ulong userId);

    /// <summary>
    /// Get session
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<SessionDTO> GetSession(ulong messageId);

    /// <summary>
    /// Delete session
    /// </summary>
    /// <param name="messageId">Message ID></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task DeleteSession(ulong messageId);
}