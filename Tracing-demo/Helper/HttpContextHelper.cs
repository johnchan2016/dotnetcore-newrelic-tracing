using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracing_demo.Helper
{
    public class HttpContextHelper
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

        public static async Task<string> GetResponseBodyStringAsync(MemoryStream responseBody, Stream originalBodyStream)
        {
            string responseContent;

            responseBody.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(responseBody))
            {
                responseContent = await reader.ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                await responseBody.CopyToAsync(originalBodyStream);
            }            

            return responseContent;
        }
    }
}
