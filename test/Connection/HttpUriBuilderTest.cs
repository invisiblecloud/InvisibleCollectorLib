using System;
using System.Collections.Generic;
using System.Text;
using InvisibleCollectorLib.Connection;
using NUnit.Framework;

namespace test.Connection
{
    [TestFixture]
    class HttpUriBuilderTest
    {
        [Test]
        public void Constructor_CorrectUri()
        {
            new HttpUriBuilder("http://ahost.adomain:4000");

            new HttpUriBuilder("https://host/");
        }

        [Test]
        public void Constructor_MalformedUri()
        {
            const string MalformedUri = "http://ahost\\.adomain:4000";
            Assert.That(() => new HttpUriBuilder(MalformedUri), Throws.Exception);
        }

        [Test]
        public void Constructor_RelativeUri()
        {
            Assert.That(() => new HttpUriBuilder("adomain"), Throws.Exception);
        }

        [Test]
        public void Constructor_NonHttpUri()
        {
            Assert.That(() => new HttpUriBuilder("ftp://host.domain"), Throws.Exception);
        }

        [Test]
        public void BuildUri_MultipleFragments()
        {
            const string BaseUri = "http://host.domain";
            const string Fragment1 = "company";
            const string Fragment2 = "123";
            var builder = new HttpUriBuilder(BaseUri);
            var result = builder.BuildUri(Fragment1, Fragment2);

            Assert.AreEqual($"{BaseUri}/{Fragment1}/{Fragment2}", result.AbsoluteUri);
        }

    }
}
