﻿using System.Collections.Concurrent;

namespace InMemoryMessaging;

public class EventQueue : IEventQueue
{
	private readonly ConcurrentQueue<object> _queue;
	private readonly BlockingCollection<object> _blockingCollection;

	public EventQueue()
	{
		_queue = new ConcurrentQueue<object>();
		_blockingCollection = new BlockingCollection<object>(_queue);
	}

	public void Enqueue(object message)
	{
		_blockingCollection.Add(message);
	}

	public object DequeueOrWait(CancellationToken cancellationToken)
	{
		return _blockingCollection.Take(cancellationToken);
	}
}