using System.Collections.Generic;
using InvisibleCollectorLib.Utils;
using NUnit.Framework;

namespace test.Utils
{
    internal class CollectionExtensionsTest
    {
        private IDictionary<string, int> BuildDict()
        {
            return new Dictionary<string, int>
            {
                {"a", 1},
                {"b", 2}
            };
        }

        [Test]
        public void StringifyDictionary_Correct()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["a"] = "test",
                ["test2"] = 0.9
            };

            var result = dictionary.StringifyDictionary();
            TestingUtils.AssertStringContainsValues(result, "a", "test", "test2", "9");
        }

        [Test]
        public void EqualsCollection_DictionaryEquality()
        {
            Assert.True(new Dictionary<string, int>().EqualsCollection(new Dictionary<string, int>()));

            var dict1 = BuildDict();
            Assert.True(dict1.EqualsCollection(dict1));

            var dict2 = new Dictionary<string, int>(dict1);
            Assert.True(dict1.EqualsCollection(dict2));
        }

        [Test]
        public void EqualsCollection_DictionaryInequality()
        {
            var dict1 = BuildDict();
            var dict2 = new Dictionary<string, int>
            {
                {"c", 3}
            };
            Assert.False(dict1.EqualsCollection(dict2));

            var dict3 = BuildDict();
            dict3["a"] = -100;

            Assert.False(dict1.EqualsCollection(dict2));
        }

        [Test]
        public void EqualsCollection_ListEquality()
        {
            Assert.True(new List<string>().EqualsCollection(new List<string>()));

            var list1 = new List<string> {"a", "b"};
            Assert.True(list1.EqualsCollection(list1));

            var list2 = new List<string>(list1);
            Assert.True(list1.EqualsCollection(list2));
        }

        [Test]
        public void EqualsCollection_ListInequality()
        {
            var list1 = new List<string> {"a", "b"};
            var list2 = new List<string> {"a"};
            Assert.False(list1.EqualsCollection(list2));

            var list3 = new List<string>(list1);
            list3[0] = "aaaa";

            Assert.False(list1.EqualsCollection(list3));
        }
    }
}