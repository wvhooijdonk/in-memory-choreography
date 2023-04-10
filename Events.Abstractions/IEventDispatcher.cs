namespace Events.Implementation
{
	public interface IEventDispatcher
	{
		Task Dispatch(object @event);
	}
}