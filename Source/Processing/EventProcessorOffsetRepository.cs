namespace Dolittle.Runtime.Events.Processing.InMemory
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using System.Collections.Concurrent;

    public class EventProcessorOffsetRepository : IEventProcessorOffsetRepository
    {
        ConcurrentDictionary<EventProcessorId,CommittedEventVersion> _lastProcessed;

        public EventProcessorOffsetRepository()
        {
            _lastProcessed = new ConcurrentDictionary<EventProcessorId, CommittedEventVersion>();
        }

        public CommittedEventVersion Get(EventProcessorId eventProcessorId)
        {
            CommittedEventVersion lastProcessedVersion;
            return _lastProcessed.TryGetValue(eventProcessorId, out lastProcessedVersion) ? lastProcessedVersion : CommittedEventVersion.None;
        }

        public void Set(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion)
        {
            _lastProcessed.AddOrUpdate(eventProcessorId,committedEventVersion,(id,v) => committedEventVersion);
        }
    }
}