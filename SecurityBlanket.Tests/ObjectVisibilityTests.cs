using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityBlanket.Tests
{
    /// <summary>
    /// An object without the proper interface
    /// </summary>
    public class InsecureObject
    {
    }

    public class VisibilityObject : IVisibleResult
    {
        private bool _allowed;
        public VisibilityObject(bool allowed) { _allowed = allowed; }
        bool IVisibleResult.IsVisible(HttpContext context)
        {
            return _allowed;
        }
    }
    public class AsyncVisibilityObject : IVisibleAsyncResult
    {
        private bool _allowed;
        public AsyncVisibilityObject(bool allowed) { _allowed = allowed; }
        Task<bool> IVisibleAsyncResult.IsVisibleAsync(HttpContext context)
        {
            return Task.FromResult(_allowed);
        }
    }

    [TestClass]
    public class ObjectVisibilityTests
    {
        [TestMethod]
        public async Task BasicVisibility()
        {
            var visibleObject = new VisibilityObject(true);
            Assert.IsTrue(await VisibilityChecker.ValidateObject(visibleObject, null));

            var insecureObject = new VisibilityObject(false);
            Assert.IsFalse(await VisibilityChecker.ValidateObject(insecureObject, null));

            var otherObject = new InsecureObject();
            Assert.IsFalse(await VisibilityChecker.ValidateObject(otherObject, null));
        }

        [TestMethod]
        public async Task CollectionVisibility()
        {
            var emptyArray = Array.Empty<VisibilityObject>();
            Assert.IsTrue(await VisibilityChecker.ValidateObject(emptyArray, null));

            var visibleArray = new VisibilityObject[] { new VisibilityObject(true) };
            Assert.IsTrue(await VisibilityChecker.ValidateObject(visibleArray, null));

            var insecureArray = new VisibilityObject[] { new VisibilityObject(false) };
            Assert.IsFalse(await VisibilityChecker.ValidateObject(insecureArray, null));

            var mixedArray = new VisibilityObject[] { new VisibilityObject(true), new VisibilityObject(true), new VisibilityObject(true), new VisibilityObject(false), };
            Assert.IsFalse(await VisibilityChecker.ValidateObject(insecureArray, null));

            var arrayWithRandomObjects = new Object[] { new VisibilityObject(true), new InsecureObject() };
            Assert.IsFalse(await VisibilityChecker.ValidateObject(arrayWithRandomObjects, null));
        }


        [TestMethod]
        public async Task DictionaryVisibility()
        {
            var emptyDict = new Dictionary<string, VisibilityObject>();
            Assert.IsTrue(await VisibilityChecker.ValidateObject(emptyDict, null));

            var visibleDict = new Dictionary<string, VisibilityObject>();
            visibleDict["key"] = new VisibilityObject(true);
            Assert.IsTrue(await VisibilityChecker.ValidateObject(visibleDict, null));

            var insecureDict = new Dictionary<string, VisibilityObject>();
            insecureDict["key"] = new VisibilityObject(false);
            Assert.IsFalse(await VisibilityChecker.ValidateObject(insecureDict, null));

            var mixedDict = new Dictionary<string, VisibilityObject>();
            mixedDict["key1"] = new VisibilityObject(true);
            mixedDict["key2"] = new VisibilityObject(false);
            Assert.IsFalse(await VisibilityChecker.ValidateObject(mixedDict, null));

            var genericDictionary = new Dictionary<string, object>();
            genericDictionary["key1"] = new VisibilityObject(true);
            genericDictionary["key2"] = new InsecureObject();
            Assert.IsFalse(await VisibilityChecker.ValidateObject(genericDictionary, null));
        }
    }
}