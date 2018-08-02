using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace test.Utils
{
    static class TestUtils
    {
        public static void AssertStringContainsValues(string containingString, params string[] values)
        {
            foreach (var value in values)
            {
                StringAssert.Contains(value, containingString);
            }
        }
    }
}
