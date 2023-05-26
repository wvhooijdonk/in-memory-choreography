namespace Events.Abstractions;

public interface IEventQueueRepository
{
    IEventQueue Get(string name);
    bool TryGet(string name, out IEventQueue queue);
}
