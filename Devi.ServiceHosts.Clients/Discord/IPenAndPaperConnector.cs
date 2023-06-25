using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.PenAndPaper;

namespace Devi.ServiceHosts.Clients.Discord;

/// <summary>
/// Pen and paper connector
/// </summary>
public interface IPenAndPaperConnector
{
    /// <summary>
    /// Refresh campaign message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RefreshCampaignMessage(RefreshCampaignMessageDTO dto);

    /// <summary>
    /// Refresh session message
    /// </summary>
    /// <param name="dto">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RefreshSessionMessage(RefreshSessionMessageDTO dto);

    /// <summary>
    /// Post log message
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <param name="channelId">Channel ID</param>
    /// <typeparam name="T">Sub type</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PostLogMessage<T>(PostLogMessageDTO<T> dto, ulong channelId);

    /// <summary>
    /// Add players to threads
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddPlayers(AddPlayersDTO dto);
}