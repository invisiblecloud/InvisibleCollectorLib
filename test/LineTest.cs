using NUnit.Framework;
using System;
using InvoiceCaptureLib;

namespace InvoiceCaptureLib
{
    [TestFixture()]
    public class LineTest
    {
        [Test()]
        public void testNumber()
        {
            const string NumberValue = "hello";

            Line line = new Line(NumberValue);
            Assert.AreEqual(line.Number, NumberValue);
        }
    }
}
