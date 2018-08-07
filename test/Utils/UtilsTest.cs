using InvisibleCollectorLib.Utils;
using NUnit.Framework;

namespace test.Utils
{
    [TestFixture]
    internal class UtilsTest
    {
        [Test]
        public void ReferenceNullableEquals_Values()
        {
            const string value = "value";
            Assert.True(IcUtils.ReferenceNullableEquals(null, null));
            Assert.True(IcUtils.ReferenceNullableEquals(value, value));
            Assert.False(IcUtils.ReferenceNullableEquals(null, value));
            Assert.False(IcUtils.ReferenceNullableEquals(value, null));
            Assert.IsNull(IcUtils.ReferenceNullableEquals(value, "other"));
        }
    }
}