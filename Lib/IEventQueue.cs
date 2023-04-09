namespace InMemoryMessaging;

public interface IEventQueue
{
	object DequeueOrWait(CancellationToken cancellationToken);
	void Enqueue(object message);
}