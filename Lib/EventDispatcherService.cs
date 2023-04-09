using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace InMemoryMessaging;

public class EventDispatcherService : BackgroundService
{
	private readonly IEventQueue _queue;
	private readonly IServiceProvider _serviceProvider;
	public EventDispatcherService(IServiceProvider serviceProvider, IEventQueue queue)
	{
		_serviceProvider = serviceProvider;
		_queue = queue;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		await Task.Yield();

		while (!cancellationToken.IsCancellationRequested)
		{
			object message = _queue.DequeueOrWait(cancellationToken);

			Type messageType = message.GetType();
			Type messageListenerType = typeof(IEventListener<>).MakeGenericType(messageType);

			using (var scope = _serviceProvider.CreateScope())
			{
				IEnumerable<object> messageListeners = GetMessageListenersByType(messageListenerType);
				MethodInfo method = messageListenerType.GetMethod("ReceiveMessage");
				//MethodInfo genericMethod = method.MakeGenericMethod(messageType);

				foreach (var messageListener in messageListeners)
				{
					method.Invoke(messageListener, new object?[] { message });
				}
			}

		}
	}

	private List<object> GetMessageListenersByType(Type messageListenerType)
	{
		return _serviceProvider.GetServices(messageListenerType)
			.Where(l => l != null)
			.Cast<object>()
			.ToList();
	}
}
