using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Api.Client.Sample;
using Sample.Api.Client.Sample.Interface;

namespace Sample.Api.Client
{
    public  static class ServiceProvider
    {
        private const string _configKey = "";
        private const string _clientName = nameof(SampleApiClient);
        public static IHttpClientBuilder AddArcadeApiClient(this IServiceCollection service, IConfiguration configuration)
        {
            var baseAddress = configuration[_configKey];
            service.AddSingleton<ISampleApiClient, SampleApiClient>(provider =>
            {
                return new SampleApiClient(provider.GetService<IHttpClientFactory>().CreateClient(_clientName));
            });

            return service.AddHttpClient(_clientName, client => client.BaseAddress = new Uri($"{baseAddress}"));
        }
    }
}
