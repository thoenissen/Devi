using Devi.EventQueue.Configurations;
using Devi.EventQueue.Events.Data;

namespace Devi.EventQueue.Events.Configurations
{
    /// <summary>
    /// Posting reminder messages
    /// </summary>
    public class PostReminderEventQueueConfiguration : EventQueueConfiguration<PostReminderEventData>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PostReminderEventQueueConfiguration()
            : base(nameof(PostReminderEventQueueConfiguration))
        {
            Persistent = true;
            PrefetchCount = 10;
            ConsumerCounter = 10;
        }

        #endregion // Constructor
    }
}