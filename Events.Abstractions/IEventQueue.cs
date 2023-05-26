namespace Events.Abstractions;

public interface IEventQueue
{
	object DequeueOrWait(CancellationToken cancellationToken);
	void Enqueue(string routingKey, object @event);
}