namespace Events.Abstractions;

public interface IEventHandler<T> where T : class
{
	Task HandleEvent(T @event);
}
