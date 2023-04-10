namespace Events.Abstractions
{
	public interface IEventDispatcher
	{
		Task Dispatch(object @event);
	}
}