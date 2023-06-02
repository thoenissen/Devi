using System.Collections.Generic;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Set players
/// </summary>
public class SetPlayersDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Players
    /// </summary>
    public List<ulong> Players { get; set; }
}