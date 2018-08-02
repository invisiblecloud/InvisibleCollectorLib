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

            string result = InvoiceCaptureLib.Utils.Utils.StringifyDictionary(dictionary);
            StringAssert.Contains("12", result);
            StringAssert.Contains("test", result);
            StringAssert.Contains("test2", result);
            StringAssert.Contains("9", result); //contains the double
        }

    }
}
