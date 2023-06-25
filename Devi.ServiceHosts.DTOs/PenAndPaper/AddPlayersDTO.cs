using System.Collections.Generic;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Add users to threads
/// </summary>
public class AddPlayersDTO
{
    /// <summary>
    /// Overview thread ID
    /// </summary>
    public ulong OverviewThreadId { get; set; }

    /// <summary>
    /// Log thread ID
    /// </summary>
    public ulong LogThreadId { get; set; }

    /// <summary>
    /// Players
    /// </summary>
    public List<ulong> Players { get; set; }
}