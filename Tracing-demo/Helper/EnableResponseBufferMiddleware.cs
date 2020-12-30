using Microsoft.AspNetCore.Http;
using NewRelic.Api.Agent;
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
            context.Request.EnableBuffering();

            IAgent agent = NewRelic.Api.Agent.NewRelic.GetAgent();
            ITransaction transaction = agent.CurrentTransaction;

            try
            {
                if (context.Request.Method != HttpMethods.Get)
                {
                    var requestValue = await HttpContextHelper.GetRequestBodyStringAsync(context.Request);
                    transaction.CurrentSpan.AddCustomAttribute("custom.request", requestValue);
                }

                string responseContent;

                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    responseContent = await HttpContextHelper.GetResponseBodyStringAsync(responseBody, originalBodyStream);

                    transaction.CurrentSpan.AddCustomAttribute("custom.response", responseContent);
                }
            }
            catch(Exception ex)
            {
                transaction.CurrentSpan
                    .AddCustomAttribute("custom.message", ex.GetErrorMessage())
                    .AddCustomAttribute("custom.stacktrace", ex.StackTrace);
            }
        }
    }
}
