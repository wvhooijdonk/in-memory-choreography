using Events.Abstractions;
using System.Collections.Concurrent;

namespace Events.Implementation;

public class EventQueue : IEventQueue
{
	private readonly ConcurrentQueue<object> _queue;
	private readonly BlockingCollection<object> _blockingCollection;

	public EventQueue()
	{
		_queue = new ConcurrentQueue<object>();
		_blockingCollection = new BlockingCollection<object>(_queue);
	}

	public void Enqueue(string routingKey, object @event)
	{
		_blockingCollection.Add(@event);
	}

	public object DequeueOrWait(CancellationToken cancellationToken)
	{
		return _blockingCollection.Take(cancellationToken);
	}
}
