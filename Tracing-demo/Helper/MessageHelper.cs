using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracing_demo.Helper
{
    public static class MessageHelper
    {
        public static string GetErrorMessage(this Exception ex)
        {
            var msgs = GetInnerExceptions(ex).Select(e => e.Message);

            return string.Join("; ", msgs);
        }

        private static IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
