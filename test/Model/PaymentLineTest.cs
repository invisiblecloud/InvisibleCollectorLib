using InvisibleCollectorLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    public class PaymentLineTest
    {
        [Test]
        public void Clone_correctness()
        {
            const string number = "123";
            var line1 = new PaymentLine
            {
                Number = number
            };

            var line2 = line1.Clone();
            Assert.AreEqual(line1, line2);
        }
    }
}