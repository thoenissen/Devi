using System;

namespace Devi.ServiceHosts.DTOs.Reminders;

/// <summary>
/// Reminder creation
/// </summary>
public class CreateOneTimeReminderDTO
{
    #region Properties

    /// <summary>
    /// Id of the user
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Id of the channel
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Timestamp of the reminder
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }

    #endregion // Properties
}