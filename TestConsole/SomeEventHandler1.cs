using Events.Abstractions;

namespace InMemoryMessaging;

public class SomeEventHandler1 : IEventHandler<SomeEvent>
{
	public async Task HandleEvent(SomeEvent @event)
	{
		Console.WriteLine($"{this.GetType().Name} received: {@event.Text}");
	}
}
