using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Events;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store.InMemory
{
    /// <summary>
    /// An InMemory implementation of an <see cref="IEventStore" />
    /// This should never be used as anything other than a testing tool
    /// </summary>
    public class EventStore : IEventStore
    {
        EventStreamCommitterAndFetcher _event_committer_and_fetcher;

        /// <summary>
        /// Instantiates a new instance of the <see cref="EventStore" />
        /// </summary>
        public EventStore()
        {
            _event_committer_and_fetcher = new EventStreamCommitterAndFetcher();
        }

        /// <inheritdoc />
        public CommittedEventStream Commit(UncommittedEventStream uncommittedEvents)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.Commit(uncommittedEvents);
        }

        void ThrowIfDisposed()
        {
            if(!IsDisposed){
                return;
            }
            throw new ObjectDisposedException("InMemoryEventStore is already disposed");
        }

        /// <summary>
        /// Disposes of the <see cref="EventStore" />
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Indicates whether the <see cref="EventStore" /> has been disposed.
        /// </summary>
        /// <value>true if disposed, false otherwise</value>
        public bool IsDisposed
        {
            get; private set;
        }

        void Dispose(bool disposing)
        {
            IsDisposed = true;
        }
        
        /// <inheritdoc />
        public Commits Fetch(EventSourceId eventSourceId)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.Fetch(eventSourceId);
        }

        /// <inheritdoc />
        public Commits FetchFrom(EventSourceId eventSourceId, CommitVersion commitVersion)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchFrom(eventSourceId,commitVersion);
        }

         /// <inheritdoc />
        public Commits FetchAllCommitsAfter(CommitSequenceNumber commit)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchAllCommitsAfter(commit);
        }
        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfType(ArtifactId eventType) 
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchAllEventsOfType(eventType);
        }
        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfTypeAfter(ArtifactId eventType, CommitSequenceNumber commit)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchAllEventsOfTypeAfter(eventType,commit);
        }
        /// <inheritdoc />
        public EventSourceVersion GetCurrentVersionFor(EventSourceId eventSource)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.GetCurrentVersionFor(eventSource);
        }
        /// <inheritdoc />
        public EventSourceVersion GetNextVersionFor(EventSourceId eventSource)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.GetNextVersionFor(eventSource);
        }
    }
}