using Events.Abstractions;

namespace Events.Implementation;

public class EventPublisher : IEventPublisher
{
	private readonly IEventExchange _exchange;

	public EventPublisher(IEventExchange exchange)
	{
		_exchange = exchange;
	}

	public void Publish(string routingKey, object @event)
	{
		Console.WriteLine($"Publishing event: {routingKey}");
        _exchange.Distribute(routingKey, @event);
	}
}
