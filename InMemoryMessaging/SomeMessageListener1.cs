namespace InMemoryMessaging;

public class SomeMessageListener1 : IEventListener<SomeMessage>
{
	public void ReceiveMessage(SomeMessage message)
	{
		Console.WriteLine($"Message received1: {message.Text}");
	}
}
