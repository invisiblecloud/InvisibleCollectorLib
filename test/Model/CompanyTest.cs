using NUnit.Framework;

namespace InvoiceCaptureLib.Model
{
    [TestFixture()]
    public class CompanyTest
    {
        [Test()]
        public void Properties_savesFieldsCorrectly()
        {
            const string CompanyName = "hello";
            const string Id = null;
            bool? notifications = false;

            Company company = new Company();
            company.Name = CompanyName;
            company.Id = Id;
            company.NotificationsEnabled = notifications;
            Assert.AreEqual(company.Name, CompanyName);
            Assert.AreEqual(company.Id, Id);
            Assert.AreEqual(company.NotificationsEnabled, notifications);
        }

        [Test()]
        public void UnsetProperties_correctlyUnset()
        {
            const string CompanyName = "hello";
            const string Id = null;
            bool? notifications = false;

            Company company = new Company();
            company.Name = CompanyName;
            company.Id = Id;
            company.NotificationsEnabled = notifications;

            company.UnsetName();
            company.UnsetId();
            company.UnsetNotifications();

            Assert.IsNull(company.Name);
            Assert.IsNull(company.Name);
            Assert.IsNull(company.NotificationsEnabled);
        }
    }
}
