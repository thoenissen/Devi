using System;
using System.Collections.Generic;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Current session data
/// </summary>
public class CurrentSessionDTO
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Thread ID
    /// </summary>
    public ulong ThreadId { get; set; }

    /// <summary>
    /// Dungeon master user ID
    /// </summary>
    public ulong DungeonMasterUserId { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    public DateTime? SessionTimeStamp { get; set; }

    /// <summary>
    /// Registrations
    /// </summary>
    public List<SessionRegistrationDTO> Registrations { get; set; }
}