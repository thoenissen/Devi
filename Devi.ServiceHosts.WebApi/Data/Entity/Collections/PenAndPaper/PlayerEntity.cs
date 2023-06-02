using Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;

/// <summary>
/// Player
/// </summary>
public class PlayerEntity
{
    /// <summary>
    /// ID
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