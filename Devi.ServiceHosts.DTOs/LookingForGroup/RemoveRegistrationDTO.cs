namespace Devi.ServiceHosts.DTOs.LookingForGroup;

/// <summary>
/// Remove registration
/// </summary>
public class RemoveRegistrationDTO
{
    /// <summary>
    /// Appointment message ID
    /// </summary>
    public ulong AppointmentMessageId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }
}