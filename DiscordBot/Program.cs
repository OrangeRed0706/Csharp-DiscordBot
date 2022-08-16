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
    //services.AddLogging(loggingBuilder =>
    //{
    //    loggingBuilder.AddSerilog(dispose: true);
    //    loggingBuilder.AddConfiguration(config);
    //});
    services.AddSingleton<BasicBot>();
    services.AddSingleton<SettingsHelper>();

});
var host = builder.Build();
{
    using var serviceScope = host.Services.CreateScope();
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
        //.ConfigureLogging(delegate(HostBuilderContext hostingContext, ILoggingBuilder logging)
        //{
        //    bool flag2 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        //    if (flag2)
        //    {
        //        logging.AddFilter<EventLogLoggerProvider>((LogLevel level) => level >= LogLevel.Warning);
        //    }
        //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //    if (hostingContext.HostingEnvironment.IsDevelopment())
        //    {
        //        logging.AddConsole();
        //        logging.AddDebug();
        //    }

        //    logging.AddEventSourceLogger();
        //    if (flag2)
        //    {
        //        logging.AddEventLog();
        //    }
        //})
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