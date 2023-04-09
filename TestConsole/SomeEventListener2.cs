using Events.Abstractions;

namespace InMemoryMessaging;

public class SomeEventListener2 : IEventListener<SomeEvent>
{
	public async Task HandleEvent(SomeEvent @event)
	{
		Console.WriteLine($"{this.GetType().Name} received: {@event.Text}");
	}
}
