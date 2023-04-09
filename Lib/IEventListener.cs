namespace InMemoryMessaging;

public interface IEventListener<T> where T : class
{
	public void ReceiveMessage(T message);
}
