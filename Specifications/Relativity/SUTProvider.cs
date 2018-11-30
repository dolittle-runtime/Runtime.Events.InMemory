using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Relativity.Specs
{
    public class SUTProvider : IProvideGeodesics
    {
        public IGeodesics Build() => new InMemory.Geodesics();
    }
}