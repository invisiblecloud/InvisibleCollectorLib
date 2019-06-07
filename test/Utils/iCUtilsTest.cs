using InvisibleCollectorLib.Utils;
using NUnit.Framework;

namespace test.Utils
{
    [TestFixture]
    internal class IcUtilsTest
    {
        [Test]
        public void ReferenceNullableEquals_Values()
        {
            const string value = "value";
            Assert.True(IcUtils.ReferenceQuality(null, null));
            Assert.True(IcUtils.ReferenceQuality(value, value));
            Assert.False(IcUtils.ReferenceQuality(null, value));
            Assert.False(IcUtils.ReferenceQuality(value, null));
            Assert.IsNull(IcUtils.ReferenceQuality(value, "other"));
        }
    }
}