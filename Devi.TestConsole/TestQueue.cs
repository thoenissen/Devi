using Devi.EventQueue.Configurations;
using Devi.EventQueue.Core;

namespace Devi.TestConsole
{
    public class TestQueuePublisher : EventQueuePublisher<TestQueue, string>
    {

    }

    public class TestQueue : EventQueueConfiguration<string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TestQueue()
            : base(nameof(TestQueue))
        {
        }
    }

    public class TestQueueSubscriber : EventQueueSubscriber<TestQueue, string>
    {
        #region Overrides of EventQueueSubscriber<TestQueue,string>

        /// <summary>
        /// Execute event
        /// </summary>
        /// <param name="data">Data</param>
        protected override void Execute(string? data)
        {
            Console.WriteLine(data);
        }

        #endregion
    }  
}