namespace Devi.EventQueue.Events.Data
{
    /// <summary>
    /// Posting reminder data
    /// </summary>
    public class PostReminderEventData
    {
        /// <summary>
        /// User ID
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// Channel ID
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string? Message { get; set; }
    }
}