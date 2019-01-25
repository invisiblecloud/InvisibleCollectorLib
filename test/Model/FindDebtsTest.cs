using System;
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
                FromDate = null
            };



            var dictionary = findDebts.SendableStringDictionary;
            TestingUtils.AssertDictionaryContainsItems(dictionary,
                    ("number", "123"),
                    ("to_date", "2010-01-01"),
                    ("from_date", "")
                );
        }
        
        
    }
}