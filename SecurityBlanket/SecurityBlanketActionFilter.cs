using Microsoft.AspNetCore.Mvc.Filters;
using SecurityBlanket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SecurityBlanket.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityBlanket
{
    /// <summary>
    /// Inspects all results to be returned via the API for security.
    /// 
    /// If an object fails a security test, throws and logs it.
    /// </summary>
    public class SecurityBlanketActionFilter : IAsyncActionFilter
    {
        private ILogger<SecurityBlanketActionFilter> _logger;

        public SecurityBlanketActionFilter(ILogger<SecurityBlanketActionFilter> logger) 
        {
            this._logger = logger;
        }

        /// <summary>
        /// Test a response and ensure that it passes security rules before returning it to the caller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="VisibilityError"></exception>
        /// <exception cref="InsecureApiError"></exception>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Execute the action
            var resultContext = await next();

            // Test the result to make sure it passes security policies
            await Validator.ValidateIActionResult(resultContext.Result, context.HttpContext);

            // Flag the response so we know that visibility has been checked
            // You can examine this to make sure that security blanket is working as advertised
            context.HttpContext.Response.Headers.Add("Security", "SB");
        }

    }
}
