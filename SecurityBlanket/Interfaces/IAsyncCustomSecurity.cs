using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SecurityBlanket.Interfaces
{
    /// <summary>
    /// Represents an object that can be tested to determine if it should be visible to the 
    /// user who made an API call.
    /// </summary>
    public interface IAsyncCustomSecurity
    {
        /// <summary>
        /// Returns true if this object should be visible to the user who owns the HttpContext.
        /// </summary>
        /// <param name="context">The HttpContext for the current API call.</param>
        /// <returns>True if the object is permitted to be shown to this HttpContext.</returns>
        Task<bool> IsVisibleAsync(HttpContext context);
    }
}
