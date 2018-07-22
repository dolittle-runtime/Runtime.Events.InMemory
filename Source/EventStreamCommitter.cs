using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Events;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Store.InMemory
{
    /// <summary>
    /// Manages the committing and fetching of event streams for the <see cref="EventStore" />
    /// </summary>
    public class EventStreamCommitterAndFetcher : ICommitEventStreams, IFetchCommittedEvents
    {
        private readonly object lock_object = new object();

        private readonly List<CommittedEventStream> _commits = new List<CommittedEventStream>();
        private readonly HashSet<CommitId> _duplicates = new HashSet<CommitId>();
        private readonly HashSet<VersionedEventSource> _versions = new HashSet<VersionedEventSource>();
        private readonly ConcurrentDictionary<EventSourceId,EventSourceVersion> _currentVersions = new ConcurrentDictionary<EventSourceId,EventSourceVersion>();

        private ulong _sequenceNumber = 0;

        /// <summary>
        /// Increments the count of commits
        /// </summary>
        /// <returns>The number of commits</returns>
        public ulong IncrementCount()
        {
            lock(lock_object)
            {
                return ++_sequenceNumber;
            }
        }

        /// <inheritdoc />
        public CommittedEventStream Commit(UncommittedEventStream uncommittedEvents)
        {
            return Commit(uncommittedEvents,IncrementCount());
        }

        /// <inheritdoc />
        CommittedEventStream Commit(UncommittedEventStream uncommittedEvents, CommitSequenceNumber commitSequenceNumber)
        {
            lock(lock_object)
            {
                ThrowIfDuplicate(uncommittedEvents.Id);
                ThrowIfConcurrencyConflict(uncommittedEvents.Source);

                var commit = new CommittedEventStream(commitSequenceNumber, uncommittedEvents.Source, uncommittedEvents.Id, uncommittedEvents.CorrelationId, uncommittedEvents.Timestamp, uncommittedEvents.Events);
                _commits.Add(commit);
                _duplicates.Add(commit.Id);
                _versions.Add(commit.Source);
                return commit;
            }
        }

        void UpdateVersion(CommittedEventStream committedEvents)
        {
            _currentVersions.AddOrUpdate(committedEvents.Source.EventSource, committedEvents.Source.Version, (key, value) => committedEvents.Source.Version);
        }

        void ThrowIfDuplicate(CommitId commitId)
        {
            if (!_duplicates.Contains(commitId))
                return;

            throw new CommitIsADuplicate();
        }

        void ThrowIfConcurrencyConflict(VersionedEventSource version)
        {
            if (_versions.Contains(version) || _versions.Any(v => v.EventSource == version.EventSource && v.Version.Commit >= version.Version.Commit))
            {
                throw new EventSourceConcurrencyConflict();
            }
        }

        /// <inheritdoc />
        public CommittedEvents Fetch(EventSourceId eventSourceId)
        {
            return new CommittedEvents(_commits.Where(c => c.Source.EventSource == eventSourceId).ToList());
        }

        /// <inheritdoc />
        public CommittedEvents FetchFrom(EventSourceId eventSourceId, CommitVersion commitVersion)
        {
            return new CommittedEvents(_commits.Where(c => c.Source.EventSource == eventSourceId && c.Source.Version.Commit >= commitVersion).ToList());
        }

        /// <inheritdoc />
        public CommittedEvents FetchAllCommitsAfter(CommitSequenceNumber commit)
        {
            return new CommittedEvents(_commits.Where(c => c.Sequence > commit).ToList());
        }
    }
}