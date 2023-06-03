using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Player
/// </summary>
public class PlayerDTO
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Character name
    /// </summary>
    public string CharacterName { get; set; }

    /// <summary>
    /// Class
    /// </summary>
    public Class? Class { get; set; }
}