using Events.Abstractions;

namespace InMemoryMessaging;

public class SomeEventListener1 : IEventListener<SomeEvent>
{
	public async Task HandleEvent(SomeEvent message)
	{
		Console.WriteLine($"{this.GetType().Name} received: {message.Text}");
	}
}
