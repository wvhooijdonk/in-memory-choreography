namespace InMemoryMessaging;

public class SomeMessageListener2 : IEventListener<SomeMessage>
{
	public void ReceiveMessage(SomeMessage message)
	{
		Console.WriteLine($"Message received2: {message.Text}");
	}
}
