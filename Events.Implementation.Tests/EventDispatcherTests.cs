using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Implementation.Tests;

public partial class EventDispatcherTests 
{
	[Fact]
	public async Task Dispatch_Event_Without_Handlers()
	{
		Given_Dispatcher_Logger();
		Given_Event();

		await When_Dispatched();

		Then_Dispatcher_Logs_It_Cannot_Find_Handlers();
	}

	[Fact]
	public async Task Dispatch_Event_With_Two_Handlers()
	{
		Given_Dispatcher_Logger();
		Given_Handlers();
		Given_Event();

		await When_Dispatched();

		Then_Dispatcher_Logging_Contains_No_Text();
		Then_All_EventHandlers_Are_Called();
	}

}