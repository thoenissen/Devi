using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Add character
/// </summary>
public class AddCharacterDTO
{
    /// <summary>
    /// Channel ID
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string CharacterName { get; set; }

    /// <summary>
    /// Class
    /// </summary>
    public Class CharacterClass { get; set; }
}