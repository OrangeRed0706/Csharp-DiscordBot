using DiscordBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateDefaultBuilder().Build();
using var serviceScope = host.Services.CreateScope();
serviceScope.ServiceProvider.GetRequiredService<BasicBot>().MainAsync().GetAwaiter().GetResult();
host.Run();

IHostBuilder CreateDefaultBuilder()
{
    return Host.CreateDefaultBuilder().ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json");
    })
        .ConfigureServices(delegate (IServiceCollection service)
    {
        service.AddSingleton<BasicBot>();
        service.AddSingleton<SettingsHelper>();
    });
}