using System.Threading.Tasks;

using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.Reminders;

using Discord;

using Microsoft.AspNetCore.Mvc;

namespace Devi.ServiceHosts.Discord.Controller;

/// <summary>
/// Reminders controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class RemindersController : LocatedControllerBase
{
    #region Fields

    /// <summary>
    /// Discord client
    /// </summary>
    private readonly DiscordClient _discordClient;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="discordClient">Discord client</param>
    /// <param name="localizationService">Localization service</param>
    public RemindersController(DiscordClient discordClient, LocalizationService localizationService)
        : base(localizationService)
    {
        _discordClient = discordClient;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Post reminder message
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    public async Task<IActionResult> PostReminderMessage([FromBody]PostReminderMessageDTO data)
    {
        var channel = await _discordClient.Client.GetChannelAsync(data.ChannelId).ConfigureAwait(false);
        if (channel is IMessageChannel textChannel)
        {
            var user = await _discordClient.Client.GetUserAsync(data.UserId).ConfigureAwait(false);

            await textChannel.SendMessageAsync(string.IsNullOrWhiteSpace(data.Message)
                                                   ? LocalizationGroup.GetFormattedText("EmptyReminder", "{0} Reminder", user.Mention)
                                                   : data.Message.Contains("\n")
                                                       ? LocalizationGroup.GetFormattedText("MultiLineReminder", "{0} Reminder:\n\n{1}", user.Mention, data.Message)
                                                       : LocalizationGroup.GetFormattedText("SingleLineReminder", "{0} Reminder: {1}", user.Mention, data.Message))
                             .ConfigureAwait(false);
        }

        return Ok();
    }

    #endregion // Methods
}