using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tracing_demo.Helper
{
    public class EnableResponseBufferMiddleware
    {
        private readonly RequestDelegate _next;

        public EnableResponseBufferMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            {
                string responseContent;

                var originalBodyStream = context.Response.Body;
                using (var fakeResponseBody = new MemoryStream())
                {
                    context.Response.Body = fakeResponseBody;

                    await _next(context);

                    fakeResponseBody.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(fakeResponseBody))
                    {
                        responseContent = await reader.ReadToEndAsync();
                        fakeResponseBody.Seek(0, SeekOrigin.Begin);

                        await fakeResponseBody.CopyToAsync(originalBodyStream);
                    }
                }
                Console.WriteLine($"Response.Body={responseContent}");
            }
        }
    }
}
