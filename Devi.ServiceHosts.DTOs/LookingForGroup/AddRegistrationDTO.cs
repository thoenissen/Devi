namespace Devi.ServiceHosts.DTOs.LookingForGroup;

/// <summary>
/// Add registration
/// </summary>
public class AddRegistrationDTO
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