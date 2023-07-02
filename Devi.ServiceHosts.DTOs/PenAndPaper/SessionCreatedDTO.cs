using System;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Session created
/// </summary>
public class SessionCreatedDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    /// Time stamp
    /// </summary>
    public DateTime TimeStamp { get; set; }
}