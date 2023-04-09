﻿using Events.Abstractions;
using Events.Implementation;
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
		SendAndReceiveEvents(host);

		lifetime.StopApplication();
		await host.WaitForShutdownAsync();
	}

	private static void SendAndReceiveEvents(IHost host)
	{
		IEventPublisher eventPublisher = host.Services.GetRequiredService<IEventPublisher>();
		eventPublisher.Publish(new SomeEvent() { Text = "Some message" });
		Console.WriteLine("Press any key to quit");
		Console.ReadKey();
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
			  services.AddScoped<IEventListener<SomeEvent>, SomeEventListener1>();
			  services.AddScoped<IEventListener<SomeEvent>, SomeEventListener2>();
			  services.AddHostedService<EventDispatcherService>();
		  });

}