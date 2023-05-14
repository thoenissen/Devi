namespace Devi.ServiceHosts.DTOs.Reminders;

/// <summary>
/// Post a reminder message
/// </summary>
public class PostReminderMessageDTO
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }
}