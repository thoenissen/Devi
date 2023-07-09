using Devi.ServiceHosts.Core.Exceptions;

namespace Devi.ServiceHosts.Discord.Interaction.Exceptions;

/// <summary>
/// Discord communication with the user was aborted cause of reaching the timeout
/// </summary>
public class DiscordTimeoutException : UserMessageException
{
    #region LocatedException

    /// <summary>
    /// Returns localized message
    /// </summary>
    /// <returns>Message</returns>
    public override string GetLocalizedMessage() => LocalizationGroup.GetText("Message", "The interaction has been aborted due to reaching the timeout.");

    #endregion // LocatedException
}