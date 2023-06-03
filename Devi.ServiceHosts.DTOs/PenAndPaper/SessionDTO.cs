using System;
using System.Collections.Generic;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Session
/// </summary>
public class SessionDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Registrations
    /// </summary>
    public List<SessionRegistrationDTO> Registrations { get; set; }
}