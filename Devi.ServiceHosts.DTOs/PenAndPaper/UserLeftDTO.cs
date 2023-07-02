using System;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// User left
/// </summary>
public class UserLeftDTO
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Session time stamp
    /// </summary>
    public DateTime SessionTimeStamp { get; set; }
}