using Events.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Events.Implementation;

public class EventListenerService : BackgroundService
{
	private readonly IEventQueue _queue;
	private readonly IEventDispatcher _dispatcher;

	public EventListenerService(IEventDispatcher dispatcher, IEventQueue queue)
	{
		_dispatcher = dispatcher;
		_queue = queue;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		await Task.Yield();

		while (!cancellationToken.IsCancellationRequested)
		{
			object @event = _queue.DequeueOrWait(cancellationToken);
			await _dispatcher.Dispatch(@event);
		}
	}
}
