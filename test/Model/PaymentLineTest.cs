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
            const string Number = "123";
            var line1 = new PaymentLine
            {
                Number = Number
            };

            var line2 = line1.Clone();
            Assert.AreEqual(line1, line2);
        }
        
    }
}