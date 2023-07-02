using System;

namespace Devi.ServiceHosts.DTOs.LookingForGroup;

/// <summary>
/// Participant
/// </summary>
public class ParticipantDTO
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Registration time stamp
    /// </summary>
    public DateTime RegistrationTimeStamp { get; set; }
}