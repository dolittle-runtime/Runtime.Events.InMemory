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
    public class EventStore : IEventStore
    {
        EventStreamCommitterAndFetcher _event_committer_and_fetcher;

        public EventStore()
        {
            _event_committer_and_fetcher = new EventStreamCommitterAndFetcher();
        }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed
        {
            get; private set;
        }

        void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        public CommittedEvents Fetch(EventSourceId eventSourceId)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.Fetch(eventSourceId);
        }

        public CommittedEvents FetchFrom(EventSourceId eventSourceId, CommitVersion commitVersion)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchFrom(eventSourceId,commitVersion);
        }

        public CommittedEvents FetchAllCommitsAfter(CommitSequenceNumber commit)
        {
            ThrowIfDisposed();
            return _event_committer_and_fetcher.FetchAllCommitsAfter(commit);
        }
    }
}