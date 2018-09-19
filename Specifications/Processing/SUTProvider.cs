using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing.InMemory.Specs
{
    public class SUTProvider : IProvideTheOffsetRepository
    {
        public IEventProcessorOffsetRepository Build() => new InMemory.EventProcessorOffsetRepository();
    }
}