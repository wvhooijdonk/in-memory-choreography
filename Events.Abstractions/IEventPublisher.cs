namespace Events.Abstractions;

public interface IEventPublisher
{
	void Publish(object message);
}