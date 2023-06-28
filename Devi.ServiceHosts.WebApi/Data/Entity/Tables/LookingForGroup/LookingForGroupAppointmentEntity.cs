using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Tables.LookingForGroup;

/// <summary>
/// Looking for group appointment
/// </summary>
[Table("LookingForGroupAppointments")]
public class LookingForGroupAppointmentEntity
{
    #region Properties

    /// <summary>
    /// id of the message
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong MessageId { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Id the creator
    /// </summary>
    public ulong CreationUserId { get; set; }

    /// <summary>
    /// Id of the channel
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Id of the thread
    /// </summary>
    public ulong ThreadId { get; set; }

    #region Navigation properties

    /// <summary>
    /// Participants
    /// </summary>
    public virtual ICollection<LookingForGroupParticipantEntity> Participants { get; set; }

    #endregion // Navigation properties

    #endregion // Properties
}