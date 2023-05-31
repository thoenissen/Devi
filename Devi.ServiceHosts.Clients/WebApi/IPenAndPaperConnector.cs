using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.PenAndPaper;

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
    /// Get current session
    /// </summary>
    /// <param name="channelId">Channel ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    Task<CurrentSessionDTO> GetCurrentSession(ulong channelId);
}