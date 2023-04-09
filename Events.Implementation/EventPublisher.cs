using Events.Abstractions;

namespace Events.Implementation;

public class EventPublisher : IEventPublisher
{
	private readonly IEventQueue _queue;

	public EventPublisher(IEventQueue queue)
	{
		_queue = queue;
	}

	public void Publish(object message)
	{
		Console.WriteLine("Publishing message");
		_queue.Enqueue(message);
	}
}
