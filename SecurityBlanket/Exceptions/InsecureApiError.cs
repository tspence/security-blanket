using Microsoft.AspNetCore.Http;
using System;

namespace SecurityBlanket.Exceptions
{
    /// <summary>
    /// This error is thrown when an API returns an object that does not implement IVisibleResult or IVisibleAsyncResult
    /// </summary>
    public class InsecureApiError
    {
        public string Path { get; set; }

        public InsecureApiError(HttpContext context)
        {
            Path = context.Request.Path;
        }
    }
}
