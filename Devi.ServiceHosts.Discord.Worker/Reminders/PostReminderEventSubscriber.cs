using Devi.Core.DependencyInjection;
using Devi.EventQueue.Core;
using Devi.EventQueue.Events.Configurations;
using Devi.EventQueue.Events.Data;
using Devi.ServiceHosts.Core.Localization;

using Discord;
using Discord.Rest;

using Microsoft.Extensions.DependencyInjection;

namespace Devi.ServiceHosts.Discord.Worker.Reminders
{
    /// <summary>
    /// Posting reminder messages
    /// </summary>
    [Injectable<PostReminderEventSubscriber>(ServiceLifetime.Singleton)]
    internal class PostReminderEventSubscriber : LocatedEventQueueSubscriber<PostReminderEventQueueConfiguration, PostReminderEventData>
    {
        #region Fields

        /// <summary>
        /// Discord client
        /// </summary>
        private readonly DiscordRestClient _discordClient;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discordClient">Discord client</param>
        /// <param name="localizationService">Localization service</param>
        public PostReminderEventSubscriber(DiscordRestClient discordClient, LocalizationService localizationService)
            : base(localizationService)
        {
            _discordClient = discordClient;
        }

        #endregion // Constructor

        #region EventQueueSubscriber<PostReminderEventQueueConfiguration,PostReminderEventData>

        /// <summary>
        /// Execute event
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override async Task Execute(PostReminderEventData? data)
        {
            if (data != null)
            {
                var channel = await _discordClient.GetChannelAsync(data.ChannelId).ConfigureAwait(false);
                if (channel is IMessageChannel textChannel)
                {
                    var user = await _discordClient.GetUserAsync(data.UserId).ConfigureAwait(false);

                    await textChannel.SendMessageAsync(string.IsNullOrWhiteSpace(data.Message)
                                                           ? LocalizationGroup.GetFormattedText("EmptyReminder", "{0} Reminder", user.Mention)
                                                           : data.Message.Contains("\n")
                                                               ? LocalizationGroup.GetFormattedText("MultiLineReminder", "{0} Reminder:\n\n{1}", user.Mention, data.Message)
                                                               : LocalizationGroup.GetFormattedText("SingleLineReminder", "{0} Reminder: {1}", user.Mention, data.Message))
                                     .ConfigureAwait(false);
                }
            }
        }

        #endregion // EventQueueSubscriber<PostReminderEventQueueConfiguration,PostReminderEventData>
    }
}