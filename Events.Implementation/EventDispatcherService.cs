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
			object @event = _queue.DequeueOrWait(cancellationToken);
			await DispatchEventToListeners(@event);
		}
	}

	private async Task DispatchEventToListeners(object @event)
	{
		Type eventType = @event.GetType();
		Type eventListenerType = typeof(IEventListener<>).MakeGenericType(eventType);

		using (IServiceScope scope = _serviceProvider.CreateScope())
		{
			IEnumerable<object> eventListeners = GetEventListenersByType(scope.ServiceProvider, eventListenerType);
			MethodInfo method = eventListenerType.GetMethod(_methodName);
			if (method == null)
			{
				throw new Exception($"Method {_methodName} not found on type {eventListenerType.FullName}");
			}

			List<Task> tasks = new List<Task>();
			foreach (var eventListener in eventListeners)
			{
				Task task = (Task)method.Invoke(eventListener, new object?[] { @event })!;
				tasks.Add(task);
			}

			await Task.WhenAll(tasks.ToArray());
		}
	}

	private List<object> GetEventListenersByType(IServiceProvider serviceProvider, Type eventListenerType)
	{
		return serviceProvider.GetServices(eventListenerType)
			.Where(l => l != null)
			.Cast<object>()
			.ToList();
	}
}
