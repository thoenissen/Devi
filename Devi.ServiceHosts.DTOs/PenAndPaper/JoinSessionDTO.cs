namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Join session
/// </summary>
public class JoinSessionDTO
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