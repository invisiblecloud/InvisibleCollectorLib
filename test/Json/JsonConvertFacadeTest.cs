using System;
using System.Collections.Generic;
using InvisibleCollectorLib.Exception;
using InvisibleCollectorLib.Json;
using InvisibleCollectorLib.Model;
using NUnit.Framework;
using test.Utils;

namespace test.Json
{
    // mostly testing for correct json serialization settings
    [TestFixture]
    internal class JsonConvertFacadeTest
    {
        private const string Key1 = "key-1";
        private const string Key2 = "key-2";
        private const string Key3 = "key-3";
        private const string Key4 = "key-4";
        private const int IntVal = 11;
        private const double DecimalVal = 2.5;
        private const string NullValue = null;
        private const string StringVal = "a string";
        private const string MalformedJson = "{\\/";
        private const string DateString = "2015-05-05";
        private static readonly DateTime MinimalDate = new DateTime(2015, 5, 5);

        [Test]
        public void DictionaryToJson_IncludeNulls()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"a", null}
            };

            var json = new JsonConvertFacade().DictionaryToJson(dictionary);
            TestingUtils.AssertStringContainsValues(json, "a", "null");
        }

        [Test]
        public void JsonStreamToStringDictionary_MalformedJson()
        {
            var jsonStream = TestingUtils.StringToStream(MalformedJson);

            Assert.That(() => new JsonConvertFacade().JsonStreamToStringDictionary(jsonStream),
                Throws.InstanceOf<IcException>());
        }


        [Test]
        public void JsonStreamToStringDictionary_MultipleValues()
        {
            var pairs = new (string, object)[]
            {
                (Key1, IntVal),
                (Key2, StringVal),
                (Key3, DecimalVal),
                (Key4, NullValue)
            };

            var json = TestingUtils.BuildJson(pairs);
            var jsonStream = TestingUtils.StringToStream(json);

            var correctPairs = new[]
            {
                (Key1, IntVal.ToString()),
                (Key2, StringVal),
                (Key3, DecimalVal.ToString(TestingUtils.DotFormatter)),
                (Key4, null)
            };

            var parsedDictionary = new JsonConvertFacade().JsonStreamToStringDictionary(jsonStream);
            TestingUtils.AssertDictionaryContainsItems(parsedDictionary, correctPairs);
        }

        [Test]
        public void JsonToDictionary_Date()
        {
            var json = TestingUtils.BuildJson((Key1, DateString));
            var dict = new JsonConvertFacade().JsonToObject<Dictionary<string, object>>(json);
            TestingUtils.AssertDictionaryContainsItems(dict, (Key1, MinimalDate));
        }

        [Test]
        public void JsonToObject_IgnoreNulls()
        {
            var json = @"{""description"": null}";
            var retDict = new JsonConvertFacade().JsonToObject<Item>(json);
            Assert.AreEqual(0, retDict.Fields.Count);
        }

        [Test]
        public void ModelToSendableJson_Date()
        {
            var dict = new Dictionary<string, object> {{Key1, MinimalDate}};
            var returnedJson = new JsonConvertFacade().DictionaryToJson(dict);
            TestingUtils.AssertStringContainsValues(returnedJson, Key1, DateString);
        }

        [Test]
        public void ModelToSendableJson_DateExtraInfo()
        {
            var date = new DateTime(2015, 5, 5, 4, 4, 4);
            var dict = new Dictionary<string, object> {{Key1, date}};
            var returnedJson = new JsonConvertFacade().DictionaryToJson(dict);
            TestingUtils.AssertStringContainsValues(returnedJson, Key1, DateString);
        }
    }
}