using Microsoft.Extensions.DependencyInjection;

namespace Events.Implementation.Tests;

public abstract class TestBase
{
	protected readonly IServiceCollection Services = new ServiceCollection();
	private IServiceProvider _serviceProvider;

	public TestBase()
	{
		Services.AddLogging();
	}

	protected IServiceProvider ServiceProvider
	{
		get
		{
			_serviceProvider ??= Services.BuildServiceProvider();
			return _serviceProvider;
		}
	}
}