namespace Devi.ServiceHosts.DTOs.LookingForGroup;

/// <summary>
/// Appointment creation
/// </summary>
public class CreateAppointmentDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Creation user ID
    /// </summary>
    public ulong CreationUserId { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Thread ID
    /// </summary>
    public ulong ThreadId { get; set; }
}