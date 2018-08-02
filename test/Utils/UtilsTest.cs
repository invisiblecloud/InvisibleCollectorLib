using System.Collections.Generic;
using NUnit.Framework;
using InvoiceCaptureLib.Utils;

namespace test.Utils
{
    [TestFixture]
    class UtilsTest
    {
        [Test]
        public void StringifyDictionary_Correct()
        {
            IDictionary<object, object> dictionary = new Dictionary<object, object>();
            dictionary[12] = "test";
            dictionary["test2"] = (double) 0.9; 

            string result = InvoiceCaptureLib.Utils.IcUtils.StringifyDictionary(dictionary);
            TestUtils.AssertStringContainsValues(result, "12", "test", "test2", "9");
        }

        [Test]
        public void ReferenceNullableEquals_Values()
        {
            const string value = "value";
            Assert.True(IcUtils.ReferenceNullableEquals(null, null));
            Assert.True(IcUtils.ReferenceNullableEquals(value, value));
            Assert.False(IcUtils.ReferenceNullableEquals(null, value));
            Assert.False(IcUtils.ReferenceNullableEquals(value, null));
            Assert.IsNull(IcUtils.ReferenceNullableEquals(value, "other"));
        }

        private IDictionary<string, int> BuildDict()
        {
            return new Dictionary<string, int>()
            {
                { "a" , 1 },
                { "b", 2 }
            };
        }

        [Test]
        public void EqualsDict_Equality()
        {
            Assert.AreEqual(null, null);

            Assert.AreEqual(new Dictionary<string, int>(), new Dictionary<string, int>());

            var dict1 = BuildDict();
            Assert.AreEqual(dict1, dict1);

            var dict2 = new Dictionary<string, int>(dict1);
            Assert.AreEqual(dict1, dict2);
        }

        [Test]
        public void EqualsDict_Inequality()
        {

            var dict1 = BuildDict();
            var dict2 = new Dictionary<string, int>()
            {
                { "c", 3 }, 
            };
            Assert.AreNotEqual(dict1, dict2);

            var dict3 = BuildDict();
            dict3["a"] = -100;

            Assert.AreNotEqual(dict1, dict2);
        }


    }
}
