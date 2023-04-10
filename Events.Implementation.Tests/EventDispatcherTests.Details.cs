using Events.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Implementation.Tests;

public partial class EventDispatcherTests : TestBase
{
	private List<string> _eventHandlersCalled;
	private Mock<ILogger<EventDispatcher>> _dispatcherLoggerMock;
	private TestEvent @event;

	public EventDispatcherTests()
	{
		_eventHandlersCalled = new List<string>();
		Services.AddSingleton(sp => _eventHandlersCalled);
		Services.AddSingleton<EventDispatcher>();
	}

	private void Given_Dispatcher_Logger()
	{
		_dispatcherLoggerMock = new Mock<ILogger<EventDispatcher>>();
		Services.AddSingleton(sp => _dispatcherLoggerMock.Object);
	}

	private void Given_Event()
	{
		@event = new TestEvent { Text = "test-event" };
	}

	private async Task When_Dispatched()
	{
		EventDispatcher dispatcher = ServiceProvider.GetRequiredService<EventDispatcher>();
		await dispatcher.Dispatch(@event);
	}

	private void Then_Dispatcher_Logs_It_Cannot_Find_Handlers()
	{
		_dispatcherLoggerMock.Verify(
			m => m.Log(
				LogLevel.Debug,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("No handlers found for type")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Once());
	}

	private void Then_Dispatcher_Logging_Contains_No_Text()
	{
		_dispatcherLoggerMock.Verify(
			m => m.Log(
				It.IsAny<LogLevel>(),
				It.IsAny<EventId>(),
				It.IsAny<It.IsAnyType>(),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()),
			Times.Never);
	}

	private void Given_Handlers()
	{
		Services.AddTransient<IEventHandler<TestEvent>, TestEventHandler>();
		Services.AddTransient<IEventHandler<TestEvent>, TestEventHandler>();
	}

	private void Then_All_EventHandlers_Are_Called()
	{
		_eventHandlersCalled.Count.Should().Be(2);
	}

	public class TestEvent { 
		public string Text { get; set; }
	}

	public class TestEventHandler : IEventHandler<TestEvent>
	{
		private readonly List<string> _eventHandlersCalled;

		public TestEventHandler(List<string> eventHandlersCalled)
		{
			_eventHandlersCalled = eventHandlersCalled;
		}

		public async Task HandleEvent(TestEvent @event)
		{
			_eventHandlersCalled.Add(this.GetType().Name);
		}
	}
}