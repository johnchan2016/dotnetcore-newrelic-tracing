﻿using Microsoft.AspNetCore.Mvc;
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
        readonly IConfiguration _config;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IConfiguration config
            )
        {
            _logger = logger;
            _config = config;
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

            var host = HttpContext.Request.Host.Value.IndexOf("localhost") > 0 ? _config.GetValue<string>("HttpUrl:LocalHost") : _config.GetValue<string>("HttpUrl:Tracing");
            string baseUrl = HttpContext.Request.Scheme + "://" + host;
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

        [Route("create")]
        [HttpPost]
        public WeatherForecast Create(CreateWeatherForecastDto dto)
        {
            Log.Information($"WeatherForecast / Create / Starting up at {DateTime.Now}");

            var result = new WeatherForecast
            {
                Summary = dto.Summary,
                Date = DateTime.Now,
                TemperatureC = dto.TemperatureC
            };
            Log.Information($"WeatherForecast / Create / End Up at {DateTime.Now}");

            return result;
        }

        [Route("getdemo")]
        [HttpGet]
        public async Task<DemoDto> GetDemo()
        {
            DemoDto demo = null;
            try
            {
                Log.Information($"GetDemo  / Starting up at {DateTime.Now}");

                demo = await GetDemoAsync("/demo");

                Log.Information($"GetDemo / End Up at {DateTime.Now}");
            }
            catch(Exception ex)
            {

            }

            return demo;
        }

        async Task<DemoDto> GetDemoAsync(string path)
        {
            Log.Information($"GetDemoAsync / Start up at {DateTime.Now}");

            HttpClient client = new HttpClient();

            // Update port # in the following line.
            var host = _config.GetValue<string>("HttpUrl:Demo");
            string baseUrl = HttpContext.Request.Scheme + "://" + host;
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            DemoDto demo = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                demo = JsonConvert.DeserializeObject<DemoDto>(content);
            }

            Log.Information($"GetDemoAsync / End Up at {DateTime.Now}");
            return demo;
        }
    }
}
