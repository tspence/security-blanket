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
using System.Net.Mime;
using Newtonsoft.Json;

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

            // Validate or override the result
            resultContext.Result = await ValidateIActionResult(resultContext.Result, context.HttpContext);

            // Flag the response so we know that visibility has been checked
            // You can examine this to make sure that security blanket is working as advertised
            context.HttpContext.Response.Headers.Add("Security", "SB");
        }

        /// <summary>
        /// Inspect the async result object and throw an error if any data is not allowed to
        /// be seen by the current HttpContext.  If any objects are about to be returned via
        /// an API call that should not be seen, this method throws an exception to prevent
        /// invalid data from being returned to a customer.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <returns>void if successful; exceptions thrown on visibility problems</returns>
        public async Task<IActionResult> ValidateIActionResult(IActionResult result, HttpContext context)
        {
            switch (result)
            {
                case ObjectResult obj: 
                    var failures = await Validator.Validate(obj.Value, context);
                    if (failures.Count > 0)
                    {
                        _logger.LogError("SecurityBlanket reported {count} security error(s) in the API {path}: {failures}", new object[] { failures.Count, context.Request.Path, JsonConvert.SerializeObject(failures) });

                        // We could throw an error here, but instead we'll rewrite the response
                        return MakeError(new VisibilityError(failures, context));
                    }
                    return result;
                default:
                    _logger.LogError("SecurityBlanket detected that the API {path} returned something other than an ObjectResult", new object[] { context.Request.Path });
                    return MakeError(new InsecureApiError(result, context));
            }
        }

        public ContentResult MakeError(object error)
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(error),
                ContentType = "text/plain",
                StatusCode = 500,
            };
        }
    }
}
