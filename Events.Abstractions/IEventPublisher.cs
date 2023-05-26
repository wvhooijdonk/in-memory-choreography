namespace Events.Abstractions;

public interface IEventPublisher
{
	void Publish(string routingKey, object @event);
}