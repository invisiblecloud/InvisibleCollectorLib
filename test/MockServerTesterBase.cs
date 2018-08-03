using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using test.Connection;

namespace test
{
    class MockServerTesterBase
    {
        protected MockServerJsonFacade _mockServer;
        
        [TearDown]
        public void ResetServer()
        {
            _mockServer.Reset();
        }

        [OneTimeSetUp]
        public void StartServer()
        {
            _mockServer = new MockServerJsonFacade();
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            _mockServer.Stop();
        }
    }
}
