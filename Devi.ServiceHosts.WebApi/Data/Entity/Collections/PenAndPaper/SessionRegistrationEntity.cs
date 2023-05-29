namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;

/// <summary>
/// Registration
/// </summary>
public class SessionRegistrationEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Is the user registered to participate at the session.
    /// </summary>
    public bool IsRegistered { get; set; }
}