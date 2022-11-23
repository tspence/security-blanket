using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

            // This object will fail
            var originalResult = new ObjectResult(new ObjectWithNoSecurityPolicy());
            var finalResult = await filter.ValidateIActionResult(originalResult, context);
            Assert.AreNotEqual(originalResult, finalResult);
            var content = finalResult as ContentResult;
            Assert.IsTrue(content?.Content?.Contains("This API generated an object visibility error."));

            // Verify that we got a log error message
            mockLogger.VerifyLogging("SecurityBlanket reported 1 security error(s) in the API : [{\"Failure\":0,\"Value\":{},\"Path\":\"root\"}]", LogLevel.Error);
            mockLogger.VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ObjectWithoutPolicy()
        {
            var context = new DefaultHttpContext();
            var mockLogger = new Mock<ILogger<SecurityBlanketActionFilter>>();
            var filter = new SecurityBlanketActionFilter(mockLogger.Object);

            // This object lacks a security policy and will fail in a different manner
            var originalResult = new ObjectResult(new ObjectWithNoSecurityPolicy());
            var finalResult = await filter.ValidateIActionResult(originalResult, context);
            Assert.AreNotEqual(originalResult, finalResult);
            var content = finalResult as ContentResult;
            Assert.IsTrue(content?.Content?.Contains("This API generated an object visibility error."));

            // Verify that we got a log error message
            mockLogger.VerifyLogging("SecurityBlanket reported 1 security error(s) in the API : [{\"Failure\":0,\"Value\":{},\"Path\":\"root\"}]", LogLevel.Error);
            mockLogger.VerifyNoOtherCalls();
        }
    }
}
