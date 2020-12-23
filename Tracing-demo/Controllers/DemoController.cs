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
    public class DemoController : ControllerBase
    {

        public DemoController()
        {

        }

        public DemoDto Index()
        {
            //throw new Exception("Demo error");
            return new DemoDto
            {
                CurrentTime = DateTime.Now,
                Message = "This is HomeController.Demo()"
            };
        }
    }
}
