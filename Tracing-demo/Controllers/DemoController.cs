using Microsoft.AspNetCore.Mvc;
using System;

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
            return new DemoDto
            {
                CurrentTime = DateTime.Now,
                Message = "This is HomeController.Demo()"
            };
        }


        [Route("createperson")]
        [HttpPost]
        public void CreatePerson(CreatePersonDto dto)
        {
            throw new Exception("Person exist");
        }
    }
}
