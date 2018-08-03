using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InvoiceCaptureLib.Connection;
using NUnit.Framework;
using test.Utils;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace test.Connection
{
    [TestFixture]
    class ApiConnectionFacadeTest
    {
        private MockServerJsonFacade _mockServer;
        private const string TEST_API_KEY = "12345";

        [OneTimeSetUp]
        public void StartServer()
        {
            this._mockServer = new MockServerJsonFacade();
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            _mockServer.Stop();
        }

        [TearDown]
        public void ResetServer()
        {
            _mockServer.Reset();
        }


        [Test]
        private ApiConnectionFacade BuildApiFacade(string errorJson = null)
        {
            return new ApiConnectionFacade(TEST_API_KEY, null);
        }

        [Test]
        public async Task CallApiAsync_GetDownloadString()
        {
            const string Path = "hi";
            var json = TestingUtils.BuildJson(("a", "b"));


            _mockServer.AddRequest(Path).AddJsonResponse(json);

            var uri = _mockServer.GetUrl(Path);

            string result = await BuildApiFacade().CallApiAsync(uri, "GET");
            Assert.AreEqual(json, result);
        }
    }
}
