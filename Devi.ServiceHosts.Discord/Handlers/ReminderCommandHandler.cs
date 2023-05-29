using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Devi.ServiceHosts.Clients.WebApi;
using Devi.ServiceHosts.Core.Localization;
using Devi.ServiceHosts.Core.ServiceProvider;
using Devi.ServiceHosts.Discord.Services.Discord;
using Devi.ServiceHosts.DTOs.Reminders;

using Discord.Commands;

namespace Devi.ServiceHosts.Discord.Handlers;

/// <summary>
/// Reminder commands
/// </summary>
public class ReminderCommandHandler : LocatedServiceBase
{
    #region Fields

    /// <summary>
    /// Connector
    /// </summary>
    private readonly WebApiConnector _connector;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="localizationService">Localization service</param>
    /// <param name="connector">Connector</param>
    public ReminderCommandHandler(LocalizationService localizationService, WebApiConnector connector)
        : base(localizationService)
    {
        _connector = connector;
    }

    #endregion // Constructor

    #region Methods

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="message">Message of the reminder</param>
    /// <param name="timeSpan">Timespan until the reminder should be executed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    public async Task ReminderIn(InteractionContextContainer context, string message, string timeSpan)
    {
        var timeSpanValidation = new Regex(@"\d+(h|m|s)");
        if (timeSpanValidation.IsMatch(timeSpan))
        {
            var amount = Convert.ToUInt64(timeSpan[..^1], CultureInfo.InvariantCulture);

            var data = new CreateOneTimeReminderDTO
                                 {
                                     UserId = context.User.Id,
                                     ChannelId = context.Channel.Id,
                                     TimeStamp = timeSpan[^1..] switch
                                                 {
                                                     "h" => DateTime.Now.AddHours(amount),
                                                     "m" => DateTime.Now.AddMinutes(amount),
                                                     "s" => DateTime.Now.AddSeconds(amount),
                                                     _ => throw new InvalidOperationException()
                                                 },
                                     Message = message
                                 };

            await CreateReminder(context, data).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creation of a one time reminder
    /// </summary>
    /// <param name="context">Command context</param>
    /// <param name="message">Message of the reminder</param>
    /// <param name="date">Date</param>
    /// <param name="time">Time</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    [Command("at")]
    public async Task ReminderAt(InteractionContextContainer context, string message, string date, string time)
    {
        DateTime? timeStamp = null;

        if (string.IsNullOrWhiteSpace(date) == false)
        {
            if (new Regex(@"\d\d\d\d-\d\d-\d\d").IsMatch(date)
             && DateTime.TryParseExact(date,
                                       "yyyy-MM-dd",
                                       null,
                                       DateTimeStyles.None,
                                       out var parsedDate)
             && string.IsNullOrWhiteSpace(time) == false
             && new Regex(@"\d\d:\d\d").IsMatch(time)
             && TimeSpan.TryParseExact(time, "hh\\:mm", null, out var parsedDateTime))
            {
                timeStamp = parsedDate.Add(parsedDateTime);
            }
        }
        else
        {
            if (new Regex(@"\d\d:\d\d").IsMatch(time)
             && TimeSpan.TryParseExact(time, "hh\\:mm", null, out var parsedTime))
            {
                timeStamp = DateTime.Today.Add(parsedTime);

                if (timeStamp.Value < DateTime.Now)
                {
                    timeStamp = timeStamp.Value.AddDays(1);
                }
            }
        }

        if (timeStamp != null)
        {
            var data = new CreateOneTimeReminderDTO
                                 {
                                     UserId = context.User.Id,
                                     ChannelId = context.Channel.Id,
                                     TimeStamp = timeStamp.Value,
                                     Message = message
                                 };

            await CreateReminder(context, data).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Create Reminder
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="data">Data</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    private async Task CreateReminder(InteractionContextContainer context, CreateOneTimeReminderDTO data)
    {
        await _connector.Reminders
                        .CreateOneTimeReminder(data)
                        .ConfigureAwait(false);

        await context.ReplyAsync(LocalizationGroup.GetFormattedText("ReminderCreated",
                                                                    "The reminder has been set for {0} {1}.",
                                                                    data.TimeStamp
                                                                        .ToString(LocalizationGroup.CultureInfo.DateTimeFormat.ShortDatePattern,
                                                                                  LocalizationGroup.CultureInfo.DateTimeFormat),
                                                                    data.TimeStamp
                                                                        .ToString(LocalizationGroup.CultureInfo.DateTimeFormat.ShortTimePattern,
                                                                                  LocalizationGroup.CultureInfo.DateTimeFormat)),
                                 ephemeral: true)
                     .ConfigureAwait(false);
    }

    #endregion // Methods
}