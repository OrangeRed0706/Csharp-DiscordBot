using System.Runtime.InteropServices;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot;
using DiscordBot.Modules;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using Serilog.Events;

var builder = CreateDefaultBuilder();

builder.ConfigureServices(delegate (HostBuilderContext context, IServiceCollection services)
{
    var config = context.Configuration;
    //services.AddSingleton<BasicBot>();
    //services.AddSingleton<SettingsHelper>();
    services.AddSingleton<DiscordSocketClient>();
    services.AddSingleton<CommandService>();
    services.AddSingleton<CommandHandlingService>();
    services.AddSingleton<HttpClient>();
    services.AddSingleton<PictureService>();
    services.AddSingleton(x=> new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
    //services.BuildServiceProvider();

});

var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
{
    var services = serviceScope.ServiceProvider;
    var client = services.GetRequiredService<DiscordSocketClient>();
    client.Log += LogAsync;
    services.GetRequiredService<CommandService>().Log += LogAsync;

    await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
    await client.StartAsync();

    // Here we initialize the logic required to register our commands.
    await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

    //await Task.Delay(Timeout.Infinite);

    host.Run();
}


Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());

    return Task.CompletedTask;
}

IHostBuilder CreateDefaultBuilder()
{
    var hostBuilder = Host
        .CreateDefaultBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            IHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
            config.AddEnvironmentVariables("DOTNET_");
            config.AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true);
        })
        .UseSerilog((hostContext, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(hostContext.Configuration);
            configuration.ReadFrom.Services(services);
        })
        .UseDefaultServiceProvider(delegate (HostBuilderContext context, ServiceProviderOptions options)
        {
            bool validateOnBuild = (options.ValidateScopes = context.HostingEnvironment.IsDevelopment());
            options.ValidateOnBuild = validateOnBuild;
        });
    return hostBuilder;
}