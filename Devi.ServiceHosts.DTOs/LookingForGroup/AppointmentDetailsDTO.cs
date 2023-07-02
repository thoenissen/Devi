using System.Collections.Generic;

namespace Devi.ServiceHosts.DTOs.LookingForGroup;

/// <summary>
/// Appointment details
/// </summary>
public class AppointmentDetailsDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Participants
    /// </summary>
    public List<ParticipantDTO> Participants { get; set; }

    /// <summary>
    /// Thread ID
    /// </summary>
    public ulong ThreadId { get; set; }
}