using System;
using System.Collections.Generic;
using System.Text;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace test.Connection
{

    public class MockServerJsonFacade
    {

        private Stack<IRequestBuilder> requests = new Stack<IRequestBuilder>();
        private FluentMockServer _mockServer;


        public MockServerJsonFacade()
        {
            this._mockServer = FluentMockServer.Start();
        }

        public MockServerJsonFacade AddJsonResponse(string json = "{}", int statusCode = 200)
        {
            var request = requests.Pop();
            var response = Response.Create().WithStatusCode(statusCode).WithBody(json).WithHeader("Content-Type", "application/json");

            _mockServer.Given(request).RespondWith(response);
            return this;
        }

        private string TryPrependSlash(string value)
        {
            return value[0] != '/' ? $"/{value}" : value;
        }

        public MockServerJsonFacade AddRequest(string pathRegex)
        {
            var request = Request.Create();

            request.WithPath(TryPrependSlash(pathRegex));

            requests.Push(request);
            return this;
        }

        public Uri GetUrl(int index = 0)
        {
            return new Uri(_mockServer.Urls[0]);
        }

        public Uri GetUrl(string path, int index = 0)
        {
            return new Uri(GetUrl(index), path);
        }

        public void Reset()
        {
            _mockServer.Reset();
            requests.Clear();
        }

        public void Stop()
        {
            this._mockServer.Stop();
        }

        public static string JoinFragments(params string[] fragments)
        {
            return String.Join("/", fragments);
        }
    }
}
