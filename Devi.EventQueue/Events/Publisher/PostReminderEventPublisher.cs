using Devi.EventQueue.Core;
using Devi.EventQueue.Events.Configurations;
using Devi.EventQueue.Events.Data;

namespace Devi.EventQueue.Events.Publisher
{
    /// <summary>
    /// Publishing <see cref="PostReminderEventQueueConfiguration"/> events
    /// </summary>
    public class PostReminderEventPublisher : EventQueuePublisher<PostReminderEventQueueConfiguration, PostReminderEventData>
    {
    }
}