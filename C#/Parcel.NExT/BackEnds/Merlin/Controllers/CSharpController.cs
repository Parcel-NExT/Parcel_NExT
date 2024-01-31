using Merlin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merlin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CSharpController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CSharpController> _logger;

        public CSharpController(ILogger<CSharpController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCSharpTargets")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
