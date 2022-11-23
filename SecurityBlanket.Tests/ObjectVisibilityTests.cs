using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecurityBlanket.Tests
{
    [TestClass]
    public class ObjectVisibilityTests
    {
        private VisibilityObject _visible = new VisibilityObject(true);
        private VisibilityObject _nonvisible = new VisibilityObject(false);
        private ObjectWithNoSecurityPolicy _insecure = new ObjectWithNoSecurityPolicy();
        private NoSecurityObject _nosecurity = new NoSecurityObject();

        [TestMethod]
        public async Task BasicVisibility()
        {
            var results = await Validator.Validate(_visible, null);
            Assert.AreEqual(0, results.Count);

            results = await Validator.Validate(_nonvisible, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);

            results = await Validator.Validate(_insecure, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root", results[0].Path);
            Assert.AreEqual(FailureType.MissingPolicy, results[0].Failure);

            results = await Validator.Validate(_nosecurity, null);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public async Task CollectionVisibility()
        {
            var emptyArray = Array.Empty<VisibilityObject>();
            var results = await Validator.Validate(emptyArray, null);
            Assert.AreEqual(0, results.Count);

            var visibleArray = new object[] { new VisibilityObject(true), new NoSecurityObject() };
            results = await Validator.Validate(visibleArray, null);
            Assert.AreEqual(0, results.Count);

            var insecureArray = new object[] { new VisibilityObject(false) };
            results = await Validator.Validate(insecureArray, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[0]", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);

            var mixedArray = new object[] { new VisibilityObject(true), new VisibilityObject(true), new VisibilityObject(true), new VisibilityObject(false), };
            results = await Validator.Validate(mixedArray, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[3]", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);

            var arrayWithRandomObjects = new object[] { new VisibilityObject(true), new ObjectWithNoSecurityPolicy() };
            results = await Validator.Validate(arrayWithRandomObjects, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[1]", results[0].Path);
            Assert.AreEqual(FailureType.MissingPolicy, results[0].Failure);
        }


        [TestMethod]
        public async Task DictionaryVisibility()
        {
            var emptyDict = new Dictionary<string, VisibilityObject>();
            var results = await Validator.Validate(emptyDict, null);
            Assert.AreEqual(0, results.Count);

            var visibleDict = new Dictionary<string, object>();
            visibleDict["key"] = new VisibilityObject(true);
            visibleDict["otherKey"] = new NoSecurityObject();
            results = await Validator.Validate(visibleDict, null);
            Assert.AreEqual(0, results.Count);

            var insecureDict = new Dictionary<string, VisibilityObject>();
            insecureDict["key"] = new VisibilityObject(false);
            results = await Validator.Validate(insecureDict, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[key]", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);

            var mixedDict = new Dictionary<string, VisibilityObject>();
            mixedDict["key1"] = new VisibilityObject(true);
            mixedDict["key2"] = new VisibilityObject(false);
            results = await Validator.Validate(mixedDict, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[key2]", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);

            var genericDictionary = new Dictionary<string, object>();
            genericDictionary["key1"] = new VisibilityObject(true);
            genericDictionary["key2"] = new ObjectWithNoSecurityPolicy();
            results = await Validator.Validate(genericDictionary, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root[key2]", results[0].Path);
            Assert.AreEqual(FailureType.MissingPolicy, results[0].Failure);
        }

        [TestMethod]
        public async Task CompoundVisibility()
        {
            // Test a compound object with all valid children
            var compoundObject = new CompoundSecurityObject(true, new object[] { new VisibilityObject(true), new VisibilityObject(true) });
            var results = await Validator.Validate(compoundObject, null);
            Assert.AreEqual(0, results.Count);

            // Test a compound object with one invalid child
            var innerArray = new object[] { new VisibilityObject(true), new VisibilityObject(false) };
            compoundObject = new CompoundSecurityObject(true, innerArray);
            results = await Validator.Validate(compoundObject, null);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("root.Children[1]", results[0].Path);
            Assert.AreEqual(FailureType.FailedPolicy, results[0].Failure);
            Assert.AreEqual(innerArray[1], results[0].Value);
        }
    }
}