using Microsoft.AspNetCore.Http;
using SecurityBlanket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityBlanket.Tests
{

    /// <summary>
    /// An object without the proper interface
    /// </summary>
    public class InsecureObject
    {
    }

    public class VisibilityObject : ICustomSecurity
    {
        private bool _allowed;
        public VisibilityObject(bool allowed) { _allowed = allowed; }
        bool ICustomSecurity.IsVisible(HttpContext context)
        {
            return _allowed;
        }
    }
    public class AsyncVisibilityObject : IAsyncCustomSecurity
    {
        private bool _allowed;
        public AsyncVisibilityObject(bool allowed) { _allowed = allowed; }
        Task<bool> IAsyncCustomSecurity.IsVisibleAsync(HttpContext context)
        {
            return Task.FromResult(_allowed);
        }
    }
    public class NoSecurityObject : INoSecurity
    {

    }
}
