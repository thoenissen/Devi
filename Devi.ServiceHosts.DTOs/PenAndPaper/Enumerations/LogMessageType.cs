namespace Devi.ServiceHosts.DTOs.PenAndPaper.Enumerations;

/// <summary>
/// Log message type
/// </summary>
public enum LogMessageType
{
    /// <summary>
    /// Session created
    /// </summary>
    SessionCreated = 100,

    /// <summary>
    /// Session deleted
    /// </summary>
    SessionDeleted = 150,

    /// <summary>
    /// User joined
    /// </summary>
    UserJoined = 200,

    /// <summary>
    /// User left
    /// </summary>
    UserLeft = 250,
}