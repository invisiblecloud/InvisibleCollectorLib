using InvisibleCollectorLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]

    public class PaymentTest
    {
        [Test]
        public void AddLine_Correctness()
        {
            var item = new PaymentLine
            {
                ReferenceNumber = "1"
            };

            var debt = new Payment();
            Assert.IsNull(debt.Lines);

            debt.AddLine(item);
            Assert.AreEqual(debt.Lines.Count, 1);
            Assert.AreEqual(debt.Lines[0], item);
        }
        
        
        
    }
}