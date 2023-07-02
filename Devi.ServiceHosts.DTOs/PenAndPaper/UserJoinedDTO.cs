using System;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// User joined
/// </summary>
public class UserJoinedDTO
{
    /// <summary>
    /// User id
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Session time stamp
    /// </summary>
    public DateTime SessionTimeStamp { get; set; }
}