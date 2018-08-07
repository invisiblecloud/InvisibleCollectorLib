using System;
using System.Collections.Generic;
using System.Text;
using InvisibleCollectorLib.Utils;
using NUnit.Framework;

namespace test.Utils
{
    class DictionaryExtensionsTest
    {
        private IDictionary<string, int> BuildDict()
        {
            return new Dictionary<string, int>()
            {
                { "a" , 1 },
                { "b", 2 }
            };
        }

        [Test]
        public void StringifyDictionary_Correct()
        {
            IDictionary<object, object> dictionary = new Dictionary<object, object>
            {
                [12] = "test",
                ["test2"] = (double)0.9
            };

            string result = dictionary.StringifyDictionary();
            TestingUtils.AssertStringContainsValues(result, "12", "test", "test2", "9");
        }

        [Test]
        public void EqualsDict_Equality()
        {
            Assert.True(new Dictionary<string, int>().EqualsDict(new Dictionary<string, int>()));

            var dict1 = BuildDict();
            Assert.True(dict1.EqualsDict(dict1));

            var dict2 = new Dictionary<string, int>(dict1);
            Assert.True(dict1.EqualsDict(dict2));
        }

        [Test]
        public void EqualsDict_Inequality()
        {

            var dict1 = BuildDict();
            var dict2 = new Dictionary<string, int>()
            {
                { "c", 3 },
            };
            Assert.False(dict1.EqualsDict(dict2));

            var dict3 = BuildDict();
            dict3["a"] = -100;

            Assert.False(dict1.EqualsDict(dict2));
        }
    }
}
