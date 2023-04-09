using Events.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Events.Implementation;

public class EventDispatcherService : BackgroundService
{
	private readonly IEventQueue _queue;
	private readonly IServiceProvider _serviceProvider;
	private const string _methodName = "HandleEvent";

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
				MethodInfo method = messageListenerType.GetMethod(_methodName);
				if (method == null) {
					throw new Exception($"Method {_methodName} not found on type {messageListenerType.FullName}");
				}

				List<Task> tasks = new List<Task>();
				foreach (var messageListener in messageListeners)
				{
					Task task = (Task)method.Invoke(messageListener, new object?[] { message })!;
					tasks.Add(task);
				}

				await Task.WhenAll(tasks.ToArray());
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
