using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Tracing_demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            Log.Information("Init here");
        }

        [Route("get")]
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            Log.Information($"WeatherForecast / Get() / Starting up at {DateTime.Now}");
            var rng = new Random();

            var home = await GetHomeAsync("/home/");

            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            Log.Information($"WeatherForecast / Get() / End Up at {DateTime.Now}");

            return result;
        }

        async Task<HomeDto> GetHomeAsync(string path)
        {
            Log.Information($"GetHomeAsync / Start up at {DateTime.Now}");

            HttpClient client = new HttpClient();

            // Update port # in the following line.
            string baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HomeDto home = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                home = JsonConvert.DeserializeObject<HomeDto>(content);
            }

            Log.Information($"GetHomeAsync / End Up at {DateTime.Now}");
            return home;
        }


        [Route("error")]
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Error()
        {
            Log.Information($"WeatherForecast_v2 / Error / Starting up at {DateTime.Now}");

            var result = await GetErrorAsync("/home/");
            Log.Information($"WeatherForecast / Error / End Up at {DateTime.Now}");

            return Enumerable.Empty<WeatherForecast>();
        }

        async Task<HomeDto> GetErrorAsync(string path)
        {
            HttpClient client = new HttpClient();

            // Update port # in the following line.
            string baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            throw new Exception("HTTP Error_v2");
        }
    }
}
