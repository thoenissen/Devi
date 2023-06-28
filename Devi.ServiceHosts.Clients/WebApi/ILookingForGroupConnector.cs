using System.Threading.Tasks;

using Devi.ServiceHosts.DTOs.LookingForGroup;

namespace Devi.ServiceHosts.Clients.WebApi;

/// <summary>
/// Looking for group
/// </summary>
public interface ILookingForGroupConnector
{
    /// <summary>
    /// Create appointment
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task CreateAppointment(CreateAppointmentDTO dto);

    /// <summary>
    /// Add registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> AddRegistration(AddRegistrationDTO dto);

    /// <summary>
    /// Remove registration
    /// </summary>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> RemoveRegistration(RemoveRegistrationDTO dto);

    /// <summary>
    /// Get appointment details
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDetailsDTO> GetAppointment(ulong appointmentMessageId);

    /// <summary>
    /// Refresh appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="dto">DTO</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> RefreshAppointment(ulong appointmentMessageId, RefreshAppointmentDTO dto);

    /// <summary>
    /// Is the given user the creator of the appointment?
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> IsCreator(ulong appointmentMessageId, ulong userId);

    /// <summary>
    /// Delete appointment
    /// </summary>
    /// <param name="appointmentMessageId">Appointment message ID</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<AppointmentDTO> DeleteAppointment(ulong appointmentMessageId);
}