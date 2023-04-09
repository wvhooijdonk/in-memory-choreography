namespace InMemoryMessaging;

public interface IEventPublisher
{
	void Publish(object message);
}