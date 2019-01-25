using System.Collections.Generic;
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
            var uri = new HttpUriBuilder("http://ahost.adomain:4000/").BuildUri();
            Assert.AreEqual("http://ahost.adomain:4000/", uri.ToString());
            
            var uri2 = new HttpUriBuilder("https://host").BuildUri();
            Assert.AreEqual("https://host/", uri2.ToString());
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
        public void WithPath_MultipleFragments()
        {
            const string BaseUri = "http://host.domain";
            const string Fragment1 = "company";
            const string Fragment2 = "123";
            var builder = new HttpUriBuilder(BaseUri);
            var result = builder.WithPath(Fragment1, Fragment2).BuildUri();

            Assert.AreEqual($"{BaseUri}/{Fragment1}/{Fragment2}", result.AbsoluteUri);
        }

        [Test]
        public void WithQuery_correct()
        {
            const string BaseUri = "http://host.domain";
            var queries = new Dictionary<string, string>()
            {
                {"company", "123"},
                {"a", ""}
            };
            
            var builder = new HttpUriBuilder(BaseUri)
                .WithQuery(queries);

            foreach (var pair in queries)
            {
                StringAssert.Contains(pair.Key, builder.BuildUri().Query);
                StringAssert.Contains("=" + pair.Value, builder.BuildUri().Query);
            }
            
        }
        
    }
}
