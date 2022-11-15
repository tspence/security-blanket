using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityBlanket.Exceptions
{
    /// <summary>
    /// This error is thrown when an API returns an object that does not implement IVisibleResult or IVisibleAsyncResult
    /// </summary>
    public class InsecureApiError : Exception
    {
        /// <summary>
        /// The object that does not correctly implement IVisibleResult or IVisibleAsyncResult
        /// </summary>
        public object NonsecuredData { get; set; }
        public HttpContext Context { get; set; }

        public InsecureApiError(object nonsecuredData, HttpContext context)
        {
            NonsecuredData = nonsecuredData;
            Context = context;
        }
    }
}
