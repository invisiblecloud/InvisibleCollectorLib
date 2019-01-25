using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace test.Connection
{
    public class MockServerFacade
    {
        private readonly FluentMockServer _mockServer;

        private readonly Stack<IRequestBuilder> requests = new Stack<IRequestBuilder>();


        public MockServerFacade()
        {
            _mockServer = FluentMockServer.Start();
        }

        public MockServerFacade AddHtmlResponse(string body = "{}", int statusCode = 200)
        {
            var request = requests.Pop();
            var response = Response.Create().WithStatusCode(statusCode).WithBody(body)
                .WithHeader("Content-Type", "text/html; charset=UTF-8");

            _mockServer.Given(request).RespondWith(response);
            return this;
        }

        /**
         * Must have a previous mathing request
         */
        public MockServerFacade AddJsonResponse(string json = "{}", int statusCode = 200)
        {
            var request = requests.Pop();
            var response = Response.Create().WithStatusCode(statusCode).WithBody(json)
                .WithHeader("Content-Type", "application/json; charset=UTF-8");

            _mockServer.Given(request).RespondWith(response);
            return this;
        }

        public MockServerFacade AddRequest(string httpMethod, string pathRegex, string expectedJson = null,
            IEnumerable<(string, string)> expectedHeaders = null,
            IEnumerable<(string, string)> notExpectedHeaders = null,
            IEnumerable<string> expectedQuery = null)
        {
            var request = Request.Create();

            request.WithPath(TryPrependSlash(pathRegex))
                .UsingMethod(MatchBehaviour.AcceptOnMatch, httpMethod);

            if (!(expectedJson is null))
                request.WithBody(new JsonMatcher(expectedJson));

            if (expectedHeaders != null)
                foreach (var pair in expectedHeaders)
                    request.WithHeader(pair.Item1, $"*{pair.Item2}*", false); // should mimick String.Contains()

            if (notExpectedHeaders != null)
                foreach (var pair in notExpectedHeaders)
                    request.WithHeader(pair.Item1, $"*{pair.Item2}*", false,
                        MatchBehaviour.RejectOnMatch); // should mimick String.Contains()

            if (expectedQuery != null)
                foreach (var query in expectedQuery)
                    request.WithUrl($"*?*{query}*"); 


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

        public static string JoinFragments(params string[] fragments)
        {
            return string.Join("/", fragments);
        }

        public void Reset()
        {
            _mockServer.Reset();
            requests.Clear();
        }

        public void Stop()
        {
            _mockServer.Stop();
        }

        private string TryPrependSlash(string value)
        {
            return value[0] != '/' ? $"/{value}" : value;
        }
    }
}