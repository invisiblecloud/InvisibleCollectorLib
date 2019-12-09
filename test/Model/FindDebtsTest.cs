using System;
using System.Collections.Generic;
using InvisibleCollectorLib.Model;
using NUnit.Framework;
using test.Utils;

namespace test.Model
{
    [TestFixture]
    public class FindDebtsTest
    {
        [Test]
        public void GetSendableStringDictionary_correct()
        {
            var findDebts = new FindDebts
            {
                Number = "123",
                ToDate = new DateTime(2010, 1, 1),
                FromDate = null,
                Attributes = new Dictionary<string, string>()
                {
                    {"a", "b"}
                }
            };


            var dictionary = findDebts.SendableStringDictionary;
            TestingUtils.AssertDictionaryContainsItems(dictionary,
                ("number", "123"),
                ("to_date", "2010-01-01"),
                ("from_date", ""),
                ("attributes[a]", "b")
            );
        }
        
        [Test]
        public void EqualityOperator_AttributesCorrectness()
        {
            var number = "1234";

            var debt1 = new FindDebts() {Number = number};
            var debt2 = new FindDebts() {Number = number};

            var attributes = new Dictionary<string, string>();

            debt1.Attributes = attributes;
            debt2.Attributes = attributes;
            Assert.True(debt1 == debt2);

            attributes["a"] = "b";
            debt1.Attributes = attributes;
            debt2.Attributes = attributes;
            Assert.True(debt1 == debt2);


            attributes["a"] = "0";
            debt2.Attributes = attributes;
            Assert.False(debt1 == debt2);

            attributes.Clear();
            debt2.Attributes = attributes;
            Assert.False(debt1 == debt2);
        }
    }
}