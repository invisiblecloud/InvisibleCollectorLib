using NUnit.Framework;
using test.Connection;

namespace test
{
    // start mock server
    [SetUpFixture]
    public class MockServerSetup
    {
        public static MockServerFacade _mockServer;

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