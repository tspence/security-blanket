using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace SecurityBlanket.Exceptions
{
    /// <summary>
    /// Represents what we share to an end user
    /// </summary>
    public class VisibilityError
    {
        /// <summary>
        /// The object that should not have been seen
        /// </summary>
        public int VisibilityErrors { get; set; }
        public string Path { get; set; }
        public string Message { get { return "This API generated an object visibility error."; } }

        public VisibilityError(IEnumerable<BlanketError> errors, HttpContext context)
        {
            VisibilityErrors = errors.Count();
            Path = context.Request.Path;
        }
    }
}
