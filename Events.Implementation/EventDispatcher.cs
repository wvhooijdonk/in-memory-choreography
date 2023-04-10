using Events.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Events.Implementation;

public class EventDispatcher : IEventDispatcher
{
	private readonly IServiceProvider _serviceProvider;
	private const string _handlerMethodName = "HandleEvent";
	private readonly ILogger<EventDispatcher> _logger;

	public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public async Task Dispatch(object @event)
	{
		Type eventType = @event.GetType();
		Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

		using IServiceScope scope = _serviceProvider.CreateScope();
		IEnumerable<object> handlers = GetHandlersByType(scope.ServiceProvider, handlerType);
		if (!handlers.Any())
		{
			_logger.LogDebug($"No handlers found for type {handlerType.FullName}");
			return;
		}

		MethodInfo method = handlerType.GetMethod(_handlerMethodName);
		if (method == null)
		{
			throw new Exception($"Method {_handlerMethodName} not found on type {handlerType.FullName}");
		}

		List<Task> tasks = new List<Task>();
		foreach (var handler in handlers)
		{
			Task task = (Task)method.Invoke(handler, new object?[] { @event })!;
			tasks.Add(task);
		}

		await Task.WhenAll(tasks.ToArray());
	}

	private List<object> GetHandlersByType(IServiceProvider serviceProvider, Type eventHandlerType)
	{
		return serviceProvider.GetServices(eventHandlerType)
			.Where(l => l != null)
			.Cast<object>()
			.ToList();
	}
}
