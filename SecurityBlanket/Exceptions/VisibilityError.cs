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
        public BlanketError[] Errors { get; set; }
        public HttpContext Context { get; set; }

        public VisibilityError(BlanketError[] errors, HttpContext context)
        {
            Errors = errors;
            Context = context;
        }
    }
}
