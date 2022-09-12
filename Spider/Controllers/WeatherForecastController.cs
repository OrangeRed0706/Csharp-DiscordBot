using Microsoft.AspNetCore.Mvc;
using Spider.Services;

namespace Spider.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly PttSpiderService _pttSpiderService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,PttSpiderService pttService)
    {
        _logger = logger;
        _pttSpiderService = pttService;
    }

    [HttpGet("test")]
    public async Task<object> GetPttpost()
    {
        var test = await _pttSpiderService.Getpost();
        return test;
    }
    
    
}