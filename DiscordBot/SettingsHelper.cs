using Microsoft.Extensions.Configuration;

namespace DiscordBot
{
    public class SettingsHelper
    {
        private readonly IConfiguration _configuration;
        private string? _token;
        public string Token => _token = _configuration.GetValue<string>("token");
        public SettingsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
