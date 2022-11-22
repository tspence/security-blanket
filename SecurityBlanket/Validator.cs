using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityBlanket.Exceptions;
using SecurityBlanket.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SecurityBlanket
{
    public static class Validator
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
                    var failures = await Validate(obj.Value, context);
                    if (failures.Count > 0)
                    {
                        throw new VisibilityError(failures.ToArray(), context);
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
        public static async Task<List<BlanketError>> Validate(object item, HttpContext context)
        {
            // Maintain a queue of objects to test and a list of failures
            var queue = new Stack<Tuple<string, object>>();
            queue.Push(new Tuple<string, object>("root", item));
            List<BlanketError> results = new List<BlanketError>();

            // Iterate through each item in the queue
            while (queue.Count > 0)
            {
                var current = queue.Pop();
                switch (current.Item2)
                {
                    // Value types are considered nonsensitive since there's no way to check them.
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
                    // Items with "INoSecurity" are considered publicly visible to all.
                    case INoSecurity _:
                        break;
                    // You really shouldn't use the async security rule, but if you need to, here it is.
                    case IAsyncCustomSecurity visibleAsync:
                        if (!await visibleAsync.IsVisibleAsync(context))
                        {
                            results.Add(new BlanketError() { Failure = FailureType.FailedPolicy, Value = item, Path = current.Item1 });
                        }
                        break;
                    case ICustomSecurity visibleResult:
                        if (!visibleResult.IsVisible(context))
                        {
                            results.Add(new BlanketError() { Failure = FailureType.FailedPolicy, Value = item, Path = current.Item1 });
                        }
                        break;
                    case IDictionary dict:
                        foreach (var key in dict.Keys)
                        {
                            // Just in case the user is doing something really weird like making compound keys
                            queue.Push(new Tuple<string, object>($"{current.Item1}.Keys[{key}]", key));
                            queue.Push(new Tuple<string, object>($"{current.Item1}[{key}]", dict[key]));
                        }
                        break;
                    case IEnumerable visibleCollection:
                        int i = 0;
                        foreach (var arrayItem in visibleCollection)
                        {
                            queue.Push(new Tuple<string, object>($"{current.Item1}[{i}]", arrayItem));
                            i = i + 1;
                        }
                        break;

                    default:
                        results.Add(new BlanketError() { Failure = FailureType.MissingPolicy, Value = item, Path = current.Item1 });
                        break;
                }

                // Special case: If the object also implements ICompoundSecurity, add its children for validation
                if (current.Item2 is ICompoundSecurity compound)
                {
                    int i = 0;
                    foreach (var childItem in compound.GetChildren())
                    {
                        queue.Push(new Tuple<string, object>($"{current.Item1}.Children[{i}]", childItem));
                    }
                }
            }
            return results;
        }
    }
}
