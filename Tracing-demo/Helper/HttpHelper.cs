using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing_demo.Helper
{
    public class HttpHelper
    {
        public static async Task<string> GetRequestBodyStringAsync(HttpRequest httpRequest)
        {
            httpRequest.EnableBuffering();

            using (var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, false, 1024, true))
            {
                var body = await reader.ReadToEndAsync();

                httpRequest.Body.Seek(0, SeekOrigin.Begin);

                return body;
            }
        }

        public static async Task<string> GetResponseBodyStringAsync(HttpResponse httpResponse)
        {
            string responseContent;

            var originalBodyStream = httpResponse.Body;
            using (var fakeResponseBody = new MemoryStream())
            {
                httpResponse.Body = fakeResponseBody;

                fakeResponseBody.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(fakeResponseBody, Encoding.UTF8, false, 1024, true))
                {
                    responseContent = await reader.ReadToEndAsync();
                    fakeResponseBody.Seek(0, SeekOrigin.Begin);

                    await fakeResponseBody.CopyToAsync(originalBodyStream);
                }
            }

            return responseContent;
        }
    }
}
