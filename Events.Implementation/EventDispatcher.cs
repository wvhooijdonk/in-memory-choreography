using Events.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Events.Implementation;

public class EventDispatcher : IEventDispatcher
{
	private readonly IServiceProvider _serviceProvider;
	private const string _methodName = "HandleEvent";

	public EventDispatcher(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task Dispatch(object @event)
	{
		Type eventType = @event.GetType();
		Type eventListenerType = typeof(IEventHandler<>).MakeGenericType(eventType);

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
