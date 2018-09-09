/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/


namespace Dolittle.Runtime.Events.Processing.InMemory
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using System.Collections.Concurrent;

    /// <summary>
    /// In-Memory Implementation of <see cref="IEventProcessorOffsetRepository" />
    /// For testing purposes only
    /// </summary>
    public class EventProcessorOffsetRepository : IEventProcessorOffsetRepository
    {
        ConcurrentDictionary<EventProcessorId,CommittedEventVersion> _lastProcessed;

        /// <summary>
        /// Instantiates an instance of <see cref="EventProcessorOffsetRepository" />
        /// </summary>
        public EventProcessorOffsetRepository()
        {
            _lastProcessed = new ConcurrentDictionary<EventProcessorId, CommittedEventVersion>();
        }

        /// <inheritdoc />
        public CommittedEventVersion Get(EventProcessorId eventProcessorId)
        {
            CommittedEventVersion lastProcessedVersion;
            return _lastProcessed.TryGetValue(eventProcessorId, out lastProcessedVersion) ? lastProcessedVersion : CommittedEventVersion.None;
        }

        /// <inheritdoc />
        public void Set(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion)
        {
            _lastProcessed.AddOrUpdate(eventProcessorId,committedEventVersion,(id,v) => committedEventVersion);
        }
    }
}