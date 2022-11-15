using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityBlanket.Exceptions
{
    public class VisibilityError : Exception
    {
        /// <summary>
        /// The object that should not have been seen
        /// </summary>
        public object SensitiveData { get; set; }
        public HttpContext Context { get; set; }

        public VisibilityError(object sensitiveData, HttpContext context)
        {
            SensitiveData = sensitiveData;
            Context = context;
        }
    }
}
