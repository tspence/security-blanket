using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecurityBlanket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityBlanket.Tests
{
    [TestClass]
    public class ActionFilterTests
    {
        [TestMethod]
        public async Task ObjectWithNoSecurityAsync()
        {
            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<SecurityBlanketActionFilter>>();
            var filter = new SecurityBlanketActionFilter(mockLogger.Object);

            // Test a basic data object
            var originalResult = new ObjectResult(new NoSecurityObject());
            var finalResult = await filter.ValidateIActionResult(originalResult, context);
            Assert.AreEqual(originalResult, finalResult);
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task ObjectFailsSecurity()
        {
            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<SecurityBlanketActionFilter>>();
            var filter = new SecurityBlanketActionFilter(mockLogger.Object);

            // Test a basic data object
            var originalResult = new ObjectResult(new InsecureObject());
            var finalResult = await filter.ValidateIActionResult(originalResult, context);
            Assert.AreNotEqual(originalResult, finalResult);

            // Verify that we got a log error message
            mockLogger.Verify(v => v.LogError(It.IsAny<string>(), It.IsAny<object[]>()));
        }
    }
}
