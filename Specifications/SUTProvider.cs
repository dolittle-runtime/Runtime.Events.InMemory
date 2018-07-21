namespace Dolittle.Runtime.Events.Store.InMemory.Specs
{
    public class SUTProvider : IProvideTheEventStore
    {
        public IEventStore Build() => new InMemory.EventStore();
    }
}