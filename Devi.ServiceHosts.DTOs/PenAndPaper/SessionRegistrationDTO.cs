namespace Devi.ServiceHosts.DTOs.PenAndPaper;

/// <summary>
/// Session registration
/// </summary>
public class SessionRegistrationDTO
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Is the user registered?
    /// </summary>
    public bool IsRegistered { get; set; }
}