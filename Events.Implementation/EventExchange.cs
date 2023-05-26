using Events.Abstractions;
using Microsoft.Extensions.Logging;

namespace Events.Implementation;

public class EventExchange : IEventExchange
{
    private readonly IList<Binding> _bindings;
    private readonly IEventQueueRepository _eventQueueRepository;
    private readonly ILogger<EventExchange> _logger;

    public EventExchange(IList<Binding> bindings, IEventQueueRepository eventQueueRepository, ILogger<EventExchange> logger)
    {
        _bindings = bindings;
        _eventQueueRepository = eventQueueRepository;
        _logger = logger;
    }

    public void Distribute(string routingKey, object @event)
    {
        List<string> queueNames = GetQueueNames(routingKey);
        foreach (var queueName in queueNames)
        {
            if (!_eventQueueRepository.TryGet(queueName, out IEventQueue queue))
            {
                _logger.LogWarning($"Queue '{queueName}' was not found. Check your bindings.");
                continue;
            }
            queue.Enqueue(routingKey, @event);
        }
    }

    private List<string> GetQueueNames(string routingKey)
    {
        //for now: exact match only
        return _bindings
            .Where(binding => binding.RoutingKey == routingKey)
            .Select(binding => binding.QueueName)
            .ToList();
    }
}
