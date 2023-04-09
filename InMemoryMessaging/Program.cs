using InMemoryMessaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class Program
{
	public static async Task Main(string[] args)
	{
		using var host = CreateHostBuilder(args).Build();
		await host.StartAsync();

		var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

		IEventPublisher eventPublisher = host.Services.GetRequiredService<IEventPublisher>();
		eventPublisher.Publish(new SomeMessage() { Text = "Some message" });

		Console.WriteLine("Press any key to quit");
		Console.ReadKey();

		lifetime.StopApplication();
		await host.WaitForShutdownAsync();
	}
	private static IHostBuilder CreateHostBuilder(string[] args) =>
	  Host.CreateDefaultBuilder(args)
		  .UseConsoleLifetime()
		  .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information))
		  .ConfigureServices((hostContext, services) =>
		  {
			  services.AddSingleton(Console.Out);
			  services.AddSingleton<IEventQueue, EventQueue>();
			  services.AddScoped<IEventPublisher, EventPublisher>();
			  services.AddScoped<IEventListener<SomeMessage>, SomeMessageListener1>();
			  services.AddScoped<IEventListener<SomeMessage>, SomeMessageListener2>();
			  services.AddHostedService<EventDispatcherService>();
		  });

}