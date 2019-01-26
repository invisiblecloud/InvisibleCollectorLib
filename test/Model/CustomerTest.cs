using InvisibleCollectorLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    internal class CustomerTest
    {
        [Test]
        public void RoutableId_correctness()
        {
            var customer = new Customer();
            Assert.IsNull(customer.RoutableId);

            var id = "1234";
            customer.Gid = id;
            Assert.AreEqual(customer.RoutableId, id);

            var externalId = "0909";
            customer.ExternalId = externalId;
            Assert.AreEqual(customer.RoutableId, id);

            customer.Gid = "";
            Assert.AreEqual(customer.RoutableId, externalId);

            customer.Gid = null;
            Assert.AreEqual(customer.RoutableId, externalId);

            customer.UnsetGid();
            Assert.AreEqual(customer.RoutableId, externalId);
        }
    }
}