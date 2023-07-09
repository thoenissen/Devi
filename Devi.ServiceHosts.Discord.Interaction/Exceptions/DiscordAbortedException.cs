using Devi.ServiceHosts.Core.Exceptions;

namespace Devi.ServiceHosts.Discord.Interaction.Exceptions;

/// <summary>
/// Discord communication with the user was aborted
/// </summary>
public class DiscordAbortedException : LocatedException
{
    #region LocatedException

    /// <summary>
    /// Returns localized message
    /// </summary>
    /// <returns>Message</returns>
    public override string GetLocalizedMessage() => LocalizationGroup.GetText("Message", "The execution of the command has been aborted.");

    #endregion // LocatedException
}