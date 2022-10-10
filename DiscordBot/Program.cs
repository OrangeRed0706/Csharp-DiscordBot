using System.Runtime.InteropServices;
using DiscordBot;
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
    services.AddSingleton<BasicBot>();
    services.AddSingleton<SettingsHelper>();

});

var host = builder.Build();

using (var serviceScope = host.Services.CreateScope())
{
    serviceScope.ServiceProvider.GetRequiredService<BasicBot>().MainAsync().GetAwaiter().GetResult();
    host.Run();
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
            //configuration.WriteTo.File("logs/log-.txt", rollingInterval :RollingInterval.Day);
        })
        .UseDefaultServiceProvider(delegate (HostBuilderContext context, ServiceProviderOptions options)
        {
            bool validateOnBuild = (options.ValidateScopes = context.HostingEnvironment.IsDevelopment());
            options.ValidateOnBuild = validateOnBuild;
        });
    return hostBuilder;
}