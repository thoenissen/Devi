using System.Threading.Tasks;

using Devi.Core.DependencyInjection;
using Devi.ServiceHosts.Discord.Commands.Base;
using Devi.ServiceHosts.Discord.Handlers;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Commands.SlashCommands;

/// <summary>
/// Reminder commands
/// </summary>
[Injectable<ReminderSlashCommandModule>(ServiceLifetime.Transient)]
[DefaultMemberPermissions(GuildPermission.SendMessages)]
[Group("reminder", "Reminder creation")]
public class ReminderSlashCommandModule : SlashCommandModuleBase
{
    #region Properties

    /// <summary>
    /// Command handler
    /// </summary>
    public ReminderCommandHandler CommandHandler { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Reminder creation
    /// </summary>
    /// <param name="timeSpan">Timespan</param>
    /// <param name="message">Message</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("in", "Creation of a reminder which will executed after the given timespan.")]
    public Task ReminderIn([Summary("Timespan", "Timespan to wait to post the reminder (xh|m|s)")] string timeSpan,
                           [Summary("Message", "Message of the reminder")] string message) => CommandHandler.ReminderIn(Context, message, timeSpan);

    /// <summary>
    /// Reminder creation
    /// </summary>
    /// <param name="time">Time</param>
    /// <param name="message">Message</param>
    /// <param name="date">Date</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [SlashCommand("at", "Creation of a reminder which will executed at the given time.")]
    public Task ReminderAt([Summary("Time", "Time of the reminder (hh:mm)")] string time,
                           [Summary("Message", "Message of the reminder")] string message,
                           [Summary("Date", "Date of the reminder (yyyy-MM-dd)")] string date = null) => CommandHandler.ReminderAt(Context, message, date, time);

    #endregion // Methods
}