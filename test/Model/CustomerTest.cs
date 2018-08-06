using System;
using System.Collections.Generic;
using System.Text;
using InvoiceCaptureLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    class CustomerTest
    {
        [Test]
        public void RoutableId_correctness()
        {
            var customer = new Customer();
            Assert.IsNull(customer.RoutableId);

            string id = "1234";
            customer.Id = id;
            Assert.AreEqual(customer.RoutableId, id);

            string externalId = "0909";
            customer.ExternalId = externalId;
            Assert.AreEqual(customer.RoutableId, id);

            customer.Id = "";
            Assert.AreEqual(customer.RoutableId, externalId);

            customer.Id = null;
            Assert.AreEqual(customer.RoutableId, externalId);

            customer.UnsetId();
            Assert.AreEqual(customer.RoutableId, externalId);

        }

    }
}
