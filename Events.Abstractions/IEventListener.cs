namespace Events.Abstractions;

public interface IEventListener<T> where T : class
{
	Task HandleEvent(T @event);
}
