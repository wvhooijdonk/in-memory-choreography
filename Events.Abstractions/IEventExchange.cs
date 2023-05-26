namespace Events.Abstractions;

public interface IEventExchange
{
    void Distribute(string routingKey, object @event);
}
