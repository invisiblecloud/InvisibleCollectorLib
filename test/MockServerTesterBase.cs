using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using InvisibleCollectorLib.Utils;
using NUnit.Framework;
using test.Connection;

namespace test
{
    internal class MockServerTesterBase
    {
        protected const string TestApiKey = "12345";

        protected static readonly (string, string) ContentHeader = ("Content-Type", IcConstants.JsonMimeType);
        protected static readonly (string, string) ContentHeaderEncoding = ("Content-Type", "charset=UTF-8");
        protected static readonly (string, string) AcceptHeader = ("Accept", IcConstants.JsonMimeType);
        protected static readonly (string, string) AuthorizationHeader = ("Authorization", $"Bearer {TestApiKey}");
        protected static readonly (string, string) HostHeader = ("Host", "localhost");

        protected static readonly ImmutableList<(string, string)> BodylessHeaders =
            new List<(string, string)> {AcceptHeader, AuthorizationHeader, HostHeader}.ToImmutableList();

        protected static readonly ImmutableList<(string, string)> BodiedHeaders =
            new List<(string, string)>(BodylessHeaders) {ContentHeader, ContentHeaderEncoding}.ToImmutableList();

        protected static readonly ImmutableList<(string, string)> BodyHeaderDifference =
            BodiedHeaders.Except(BodylessHeaders).ToImmutableList();

        protected static MockServerFacade _mockServer;

        [TearDown]
        public void ResetServer()
        {
            _mockServer.Reset();
        }


        [OneTimeSetUp]
        public void StartServer()
        {
            _mockServer = new MockServerFacade();
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            _mockServer.Stop();
        }
    }
}