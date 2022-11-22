using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityBlanket.Interfaces
{
    /// <summary>
    /// Represents an object that contains nested objects within it that need to be checked for 
    /// adherence to security policies.
    /// </summary>
    public interface ICompoundSecurity
    {
        /// <summary>
        /// Implement this function to return child objects whose security policies should also
        /// be checked.  Note that you still need to implement a separate security policy for 
        /// this object.
        /// </summary>
        /// <returns>A list of child objects to check for security policy</returns>
        IEnumerable<object> GetChildren();
    }
}
