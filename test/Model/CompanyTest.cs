using NUnit.Framework;

namespace InvoiceCaptureLib.Model
{
    [TestFixture()]
    public class CompanyTest
    {
        [Test()]
        public void testNumber()
        {
            const string CompanyName = "hello";

            Company company = new Company();
            company.Name = CompanyName;
            Assert.AreEqual(company.Name, CompanyName);
        }
    }
}
