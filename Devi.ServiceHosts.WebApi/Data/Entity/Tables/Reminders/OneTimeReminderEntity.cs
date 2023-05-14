using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Tables.Reminders;

/// <summary>
/// One time reminder
/// </summary>
[Table("OneTimeReminders")]
public class OneTimeReminderEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set;  }

    /// <summary>
    /// Id of the user
    /// </summary>
    public ulong DiscordUserId { get; set; }

    /// <summary>
    /// Id of the channel
    /// </summary>
    public ulong DiscordChannelId { get; set; }

    /// <summary>
    /// Timestamp of the reminder
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Is the reminder executed?
    /// </summary>
    public bool IsExecuted { get; set; }

    #endregion // Properties
}