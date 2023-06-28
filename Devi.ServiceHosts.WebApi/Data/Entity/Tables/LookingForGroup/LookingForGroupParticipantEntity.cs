using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

/// <summary>
/// Participants of a looking for group appointment
/// </summary>
[Table("LookingForGroupParticipants")]
public class LookingForGroupParticipantEntity
{
    #region Properties

    /// <summary>
    /// Id of the appointment message
    /// </summary>
    public ulong AppointmentMessageId { get; set; }

    /// <summary>
    /// Id of the user
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Registration timestamp
    /// </summary>
    public DateTime RegistrationTimeStamp { get; set; }

    #region Navigation properties

    /// <summary>
    /// Appointment
    /// </summary>
    [ForeignKey(nameof(AppointmentMessageId))]
    public virtual LookingForGroupAppointmentEntity Appointment { get; set; }

    #endregion // Navigation properties

    #endregion // Properties
}