using Events.Abstractions;

namespace InMemoryMessaging;

public class SomeEventListener2 : IEventListener<SomeEvent>
{
	public async Task HandleEvent(SomeEvent message)
	{
		Console.WriteLine($"{this.GetType().Name} received: {message.Text}");
	}
}
