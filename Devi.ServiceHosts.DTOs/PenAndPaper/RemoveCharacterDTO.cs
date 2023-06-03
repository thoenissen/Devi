namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Remove character
/// </summary>
public class RemoveCharacterDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }
}