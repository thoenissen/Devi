namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Leave session
/// </summary>
public class LeaveSessionDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }
}