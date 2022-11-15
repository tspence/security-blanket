using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityBlanket.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SecurityBlanket
{
    public static class VisibilityChecker
    {

        /// <summary>
        /// Inspect the async result object and throw an error if any data is not allowed to
        /// be seen by the current HttpContext.  If any objects are about to be returned via
        /// an API call that should not be seen, this method throws an exception to prevent
        /// invalid data from being returned to a customer.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <returns>void if successful; exceptions thrown on visibility problems</returns>
        /// <exception cref="VisibilityError"></exception>
        /// <exception cref="InsecureApiError"></exception>
        public static async Task ValidateIActionResult(IActionResult result, HttpContext context)
        {
            switch (result)
            {
                case ObjectResult obj: 
                    var success = await ValidateObject(obj.Value, context);
                    if (!success)
                    {
                        throw new VisibilityError(obj.Value, context);
                    }
                    break;
                default:
                    throw new InsecureApiError(result, context);
            }
        }

        /// <summary>
        /// Recursively examine an object and determine whether it is permitted to be seen
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateObject(object item, HttpContext context)
        {
            var success = true;
            switch (item)
            {
                // Value types are considered nonsensitive
                case string _:
                case null:
                case bool _:
                case int _:
                case uint _:
                case short _:
                case ushort _:
                case byte _:
                case sbyte _:
                case char _:
                case long _:
                case ulong _:
                case float _:
                case double _:
                case decimal _:
                case DateTime _:
                    break;
                case IVisibleAsyncResult visibleAsync:
                    success = success && await visibleAsync.IsVisibleAsync(context);
                    break;
                case IVisibleResult visibleResult:
                    success = success && visibleResult.IsVisible(context);
                    break;
                case IDictionary dict:
                    foreach (var key in dict.Keys)
                    {
                        success = success && await ValidateObject(key, context);
                    }
                    foreach (var value in dict.Values)
                    {
                        success = success && await ValidateObject(value, context);
                    }
                    break;
                case IEnumerable visibleCollection:
                    foreach (var childItem in visibleCollection)
                    {
                        success = success && await ValidateObject(childItem, context);
                    }
                    break;

                default:
                    success = false;
                    break;
            }
            return success;
        }
    }
}
