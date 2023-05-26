using Events.Abstractions;

namespace Events.Implementation;

public class EventQueueRepository : IEventQueueRepository
{
    public IEventQueue Get(string name)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string name, out IEventQueue queue)
    {
        throw new NotImplementedException();
    }
}
