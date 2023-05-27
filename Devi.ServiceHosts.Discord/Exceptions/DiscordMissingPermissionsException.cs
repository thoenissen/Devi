using Devi.ServiceHosts.Core.Exceptions;

namespace Devi.ServiceHosts.Discord.Exceptions;

/// <summary>
/// The use doesn't have the required permission to perform this action.
/// </summary>
public class DiscordMissingPermissionsException : UserMessageException
{
    #region LocatedException

    /// <summary>
    /// Returns localized message
    /// </summary>
    /// <returns>Message</returns>
    public override string GetLocalizedMessage() => LocalizationGroup.GetText("Message", "You don't have the required permission to perform this action.");

    #endregion // LocatedException
}