using InvoiceCaptureLib.Model;
using NUnit.Framework;

namespace test.Model
{
    [TestFixture]
    public class CompanyTest
    {
        [Test]
        public void AssertHasMandatoryFields_HasAllFields()
        {
            var company = new Company
            {
                Name = "a name",
                VatNumber = "123"
            };

            company.AssertHasMandatoryFields();
        }

        [Test]
        public void AssertHasMandatoryFields_MissingFields()
        {
            var company = new Company
            {
                Name = "a name"
            };

            Assert.That(company.AssertHasMandatoryFields, Throws.Exception);
        }

        [Test]
        public void EqualityOperator_EqualCompany()
        {
            const string Name = "a name";
            const bool Notification = false;

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };

            Assert.True(company == otherCompany);
        }

        [Test]
        public void Equals_DifferentType()
        {
            var company = new Company();
            var other = "other";
            Assert.False(company.Equals(other));
        }

        [Test]
        public void Equals_EqualCompany()
        {
            const string Name = "a name";
            const bool Notification = false;

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };

            Assert.True(company.Equals(otherCompany));
        }

        [Test]
        public void Equals_Identity()
        {
            var company = new Company();
            Assert.True(company.Equals(company));
        }

        [Test]
        public void Equals_Null()
        {
            var company = new Company();
            Assert.False(company.Equals(null));
        }

        [Test]
        public void Equals_UnequalCompany()
        {
            const string Name = "a name";

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = false
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = true
            };

            Assert.False(company.Equals(otherCompany));
        }

        [Test]
        public void Equals_UnequalCompanyMoreFields()
        {
            const string Name = "a name";

            var company = new Company
            {
                Name = Name
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = true
            };

            Assert.False(company.Equals(otherCompany));
        }

        [Test]
        public void GetHashCode_EqualCompany()
        {
            const string Name = "a name";
            const bool Notification = false;

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = Notification
            };

            Assert.AreEqual(company.GetHashCode(), otherCompany.GetHashCode());
        }

        [Test]
        public void GetHashCode_UnequalCompany()
        {
            const string Name = "a name";

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = false
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = true
            };

            Assert.AreNotEqual(company.GetHashCode(), otherCompany.GetHashCode());
        }

        [Test]
        public void GetHashCode_UnequalCompanyMoreFields()
        {
            const string Name = "a name";

            var company = new Company
            {
                Name = Name
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = true
            };

            Assert.AreNotEqual(company.GetHashCode(), otherCompany.GetHashCode());
        }

        [Test]
        public void InequalityOperator_UnequalCompany()
        {
            const string Name = "a name";

            var company = new Company
            {
                Name = Name,
                NotificationsEnabled = false
            };
            var otherCompany = new Company
            {
                Name = Name,
                NotificationsEnabled = true
            };

            Assert.True(company != otherCompany);
        }

        [Test]
        public void Properties_savesFieldsCorrectly()
        {
            const string CompanyName = "hello";
            const string Id = null;
            bool? notifications = false;

            var company = new Company();
            company.Name = CompanyName;
            company.Id = Id;
            company.NotificationsEnabled = notifications;
            Assert.AreEqual(company.Name, CompanyName);
            Assert.AreEqual(company.Id, Id);
            Assert.AreEqual(company.NotificationsEnabled, notifications);
        }

        [Test]
        public void UnsetProperties_correctlyUnset()
        {
            const string CompanyName = "hello";
            const string Id = null;
            bool? notifications = false;

            var company = new Company();
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