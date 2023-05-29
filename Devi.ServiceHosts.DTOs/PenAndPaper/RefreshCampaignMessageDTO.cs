namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Refreshing campaign message
/// </summary>
public class RefreshCampaignMessageDTO
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Thread ID
    /// </summary>
    public ulong ThreadId { get; set; }

    /// <summary>
    /// Dungeon master user ID
    /// </summary>
    public ulong DungeonMasterUserId { get; set; }
}